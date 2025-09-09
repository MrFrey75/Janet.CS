using System.Text.Json.Serialization;
using Janet.Core.Enums;

namespace Janet.Core.Models;

#pragma warning disable CS8618

public record ChatMessage(
    [property: JsonPropertyName("role")] ChatMessageType Type = ChatMessageType.User,
    [property: JsonPropertyName("content")] string Content = ""
    
);

#pragma warning restore CS8618