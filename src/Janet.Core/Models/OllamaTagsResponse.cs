using System.Text.Json.Serialization;

namespace Janet.Core.Models;

public class OllamaTagsResponse
{
    [JsonPropertyName("models")]
    public List<OllamaModel> Models { get; set; } = null!;
}