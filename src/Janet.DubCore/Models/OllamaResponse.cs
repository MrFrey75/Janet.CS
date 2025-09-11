using System.Text.Json.Serialization;


namespace Janet.DubCore.Models;

public class OllamaResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    [JsonPropertyName("done")]
    public bool Done { get; set; } = false;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("intent")]
    public OllamaIntent Intent { get; set; } = new OllamaIntent()
    {
        intent = string.Empty,
        entities = new Dictionary<string, string>()
    };
}