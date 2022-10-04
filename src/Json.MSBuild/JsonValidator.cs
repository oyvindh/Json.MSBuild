namespace Json.MSBuild;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Json.Schema;
using Microsoft.Build.Framework;

/// <summary>
/// MSBuild Task for parsing and validating an arbitrary number of json files including the schema.
/// </summary>
public class JsonValidator : Microsoft.Build.Utilities.Task
{
    private static readonly HttpClient HttpClient = new HttpClient();

    [Required]
    public ITaskItem[] Files { get; set; } = Array.Empty<ITaskItem>();

    [SuppressMessage("", "VSTHRD002", Justification = "This is a task, not a service.")]
    public override bool Execute()
    {
        var validations = this.Files.Select(file =>
        {
            return this.ValidateAsync(file.ItemSpec).Result;
        });

        return !validations.Any(v => v == false);
    }

    private static async Task<Stream> DownloadAsStreamAsync(string uri)
    {
        var response = await HttpClient.GetAsync(uri).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    private async Task<bool> ValidateAsync(string filePath)
    {
        try
        {
            this.Log.LogMessage(MessageImportance.Normal, $"Validating -> {filePath}");
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Skip,
            };
            using var json = File.OpenRead(filePath);
            using var document = await JsonDocument.ParseAsync(json, options).ConfigureAwait(false);

            // Get the schema url from the json file
            if (!document.RootElement.TryGetProperty("$schema", out var schemaUrl))
            {
                this.Log.LogWarning($"The json file {filePath} does not contain a $schema property.");
                return true;
            }

            // Download the schema and use it for validation
            var schemaStream = await DownloadAsStreamAsync(schemaUrl.ToString()).ConfigureAwait(false);

            var schema = await JsonSchema.FromStream(schemaStream).ConfigureAwait(false);
            var result = schema.Validate(document.RootElement);
            var validity = result.IsValid ? "Valid" : "Invalid";

            return true;
        }
        catch (JsonException ex)
        {
            this.Log.LogErrorFromException(ex, true);
            return false;
        }
    }
}
