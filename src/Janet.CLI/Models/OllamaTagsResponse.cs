using System.Text.Json.Serialization;

namespace Janet.CLI.Models;

public class OllamaTagsResponse
{
    [JsonPropertyName("models")]
    public List<OllamaModel> Models { get; set; } = null!;
}