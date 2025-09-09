using System.Text.Json.Serialization;

namespace Janet.Core.Models;

public record IntentResponse
{
    [JsonPropertyName("intent")] public string Intent { get; init; } = "general_chat";
    [JsonPropertyName("entities")] public Dictionary<string, string>? Entities { get; init; } = new Dictionary<string, string>();
    [JsonPropertyName("confidence")] public double Confidence { get; init; } = 0.5;
}