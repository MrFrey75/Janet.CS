using System.Text.Json.Serialization;

namespace Janet.Core.Models;

public class OllamaChatResponse
{
    [JsonPropertyName("message")]
    public ChatMessage Message { get; set; } = null!;
}