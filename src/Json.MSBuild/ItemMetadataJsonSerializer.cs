namespace Json.MSBuild;

using System.Dynamic;
using System.Text.Json;
using Microsoft.Build.Framework;

/// <summary>
/// Serializes MSBuild item metadata to JSON. The identity of the item along with any custom metadata is serialized.
/// </summary>
public class ItemMetadataJsonSerializer : Microsoft.Build.Utilities.Task
{
    [Required]
    public ITaskItem[] Items { get; set; } = Array.Empty<ITaskItem>();

    public string[] MetadataToSerialize { get; set; } = Array.Empty<string>();

    [Output]
    public string Json { get; set; } = string.Empty;

    public override bool Execute()
    {
        var metadataNames = this.Items.SelectMany(i => i.MetadataNames.Cast<string>()).Distinct();
        if (!this.MetadataToSerialize.Any())
        {
            this.MetadataToSerialize = metadataNames.ToArray();
        }

        var dictionaries = metadataNames
            .Where(m => this.MetadataToSerialize.Contains(m))
            .Select(
                n => this.Items
                    .Select(i => new KeyValuePair<string, object>(n, i.GetMetadata(n)))
                    .ToDictionary(k => k.Key, v => v.Value));

        var objects = dictionaries.Select(d => DictionaryToObject(d));

        this.Json = JsonSerializer.Serialize(objects);

        return true;
    }

    private static dynamic DictionaryToObject(IDictionary<string, object> dictionary)
    {
        var expandoObj = new ExpandoObject();
        var expandoObjCollection = (ICollection<KeyValuePair<string, object>>)expandoObj;

        foreach (var keyValuePair in dictionary)
        {
            expandoObjCollection.Add(keyValuePair);
        }

        dynamic eoDynamic = expandoObj;
        return eoDynamic;
    }
}
