using System;
using Janet.DubCore.Models;
using Janet.DubCore.Enums;

namespace Janet.DubCore.Models;


public class ChatResponse
{
    public string Response { get; set; }
    public DateTime Timestamp { get; set; }

    public ChatResponse(string response)
    {
        Response = response;
        Timestamp = DateTime.UtcNow;
    }
}