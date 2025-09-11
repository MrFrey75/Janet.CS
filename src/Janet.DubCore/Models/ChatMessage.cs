using System;
using Janet.DubCore.Models;
using Janet.DubCore.Enums;

namespace Janet.DubCore.Models;

public class ChatMessage
{
    public string User { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public ChatMessageType MessageType { get; set; }

    public ChatMessage(string user, string message, ChatMessageType messageType = ChatMessageType.User)
    {
        User = user;
        Message = message;
        MessageType = messageType;
        Timestamp = DateTime.UtcNow;
    }
}