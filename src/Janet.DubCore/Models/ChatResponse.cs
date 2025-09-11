using System;
using Janet.DubCore.Models;
using Janet.DubCore.Enums;

namespace Janet.DubCore.Models;


public class ChatResponse
{
    public string Response { get; set; }
    public Dictionary<string, string> Entities { get; set; } = new Dictionary<string, string>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ChatResponse()
    {
        Response = string.Empty;
    }

    public ChatResponse(string response)
    {
        Response = response;
    }
}