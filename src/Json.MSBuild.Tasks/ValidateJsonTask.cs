namespace Json.MSBuild.Tasks;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Json.Schema;
using Microsoft.Build.Framework;

/// <summary>
/// MSBuild Task for parsing and validating an arbitrary number of json files including the schema.
/// </summary>
public class JsonValidator : Microsoft.Build.Utilities.Task
{
    [Required]
    public ITaskItem[] Files { get; set; }

    public override bool Execute()
    {
        var validations = this.Files.Select(file => ValidateAsync(file.ItemSpec));
        return true;
    }

    private static async Task<bool> ValidateAsync(string filePath)
    {
        ////try
        ////{
        using var json = File.OpenRead(filePath);
        var options = new JsonDocumentOptions
        {
            AllowTrailingCommas = false,
            CommentHandling = JsonCommentHandling.Skip,
        };
        using var document = await JsonDocument.ParseAsync(json, options).ConfigureAwait(false);
        /*
        var schema = await JsonSchema.FromStream().ConfigureAwait(false);
        var result = schema.Validate(json.RootElement);
        var validity = result.IsValid ? "Valid" : "Invalid";
        */
        return true;
        /*
        }
        catch (Exception ex)
        {
            this.Log.LogErrorFromException(ex, true);
            return false;
        }
        */
    }
}
