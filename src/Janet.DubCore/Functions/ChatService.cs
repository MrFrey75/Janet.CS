using Janet.DubCore.Models;
using Janet.DubCore.Services;

namespace Janet.DubCore.Functions;

public class ChatService
{
    private readonly ILoggingService _loggingService;

    public ChatService(ILoggingService loggingService)
    {
        _loggingService = loggingService;

        _loggingService.LogInfo("ChatService initialized.");
    }

    // Implement chat-related functionalities here

    public Task<ChatResponse> ProcessMessage(string message)
    {
        _loggingService.LogInfo($"Sending chat message: {message}");
        
        return Task.FromResult(new ChatResponse(
            $"Echo: {message}"
        )
        {
            Timestamp = DateTime.UtcNow
        });
    }
}