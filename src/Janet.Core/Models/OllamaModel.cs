using System.Text.Json.Serialization;

namespace Janet.Core.Models;

public class OllamaModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("modified_at")]
    public DateTime ModifiedAt { get; set; }
    [JsonPropertyName("size")]
    public long Size { get; set; }
}