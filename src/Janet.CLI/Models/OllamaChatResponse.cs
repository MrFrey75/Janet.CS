using System.Text.Json.Serialization;

namespace Janet.CLI.Models;

public class OllamaChatResponse
{
    [JsonPropertyName("message")]
    public ChatMessage Message { get; set; } = null!;
}