using System.Text.Json.Serialization;

namespace Janet.CLI.Models;

public class OllamaChatStreamResponse
{
    [JsonPropertyName("message")]
    public ChatMessage Message { get; set; } = null!;
    [JsonPropertyName("done")]
    public bool Done { get; set; }
}