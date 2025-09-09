using System.Text.Json.Serialization;

namespace Janet.CLI.Models;

#pragma warning disable CS8618

public record ChatMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content
);

#pragma warning restore CS8618