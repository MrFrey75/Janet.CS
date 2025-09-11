using Janet.DubCore.Functions;
using Janet.DubCore.Models;

namespace Janet.DubCore.Services;

public class Orchestrator : IOrchestrator
{
    private readonly IAppConfigService _configService;
    private readonly ILoggingService _loggingService;
    private readonly IntentService _intentService;
    private readonly ChatService _chatService;

    public Orchestrator(IAppConfigService configService, ILoggingService loggingService, IntentService intentService, ChatService chatService)
    {
        _configService = configService;
        _loggingService = loggingService;
        _intentService = intentService;
        _chatService = chatService;
        _intentService = intentService;

        _loggingService.LogInfo("Orchestrator initialized.");
    }

    public ChatResponse ProcessQuery(string query)
    {
        var chatMessage = new ChatMessage("User", query);
        chatMessage = _intentService.ProcessMessage(chatMessage).GetAwaiter().GetResult();
        query = chatMessage.Message;
        // Implement your query processing logic here

        ChatResponse response = new ChatResponse();

        if (chatMessage.Intent != null && chatMessage.Intent.intent == "error")
        {
            _loggingService.LogWarning($"Error processing query: {chatMessage.Message}.");
            response.Response = $"Error processing query: {chatMessage.Message}.";
            return response;
        }

        if (chatMessage.Intent == null || chatMessage.Intent.intent == null)
        {
            _loggingService.LogWarning("Intent or intent name is null.");
            response.Response = "Error: Unable to process intent.";
            return response;
        }

        switch (chatMessage.Intent.intent.ToLower())
        {
            case "set_timer":
                if (chatMessage.Intent.entities.TryGetValue("duration", out var duration))
                {
                    response.Response = $"Timer set for {duration}.";
                }
                else
                {
                    response.Response = "Error: 'duration' entity is missing for 'set_timer' intent.";
                }
                break;

            case "find_weather":
                if (chatMessage.Intent.entities.TryGetValue("location", out var location))
                {
                    response.Response = $"Fetching weather for {location}...";
                }
                else
                {
                    response.Response = "Error: 'location' entity is missing for 'find_weather' intent.";
                }
                break;

            default:
                _loggingService.LogWarning($"Unknown intent '{chatMessage.Intent.intent}' received.");
                return _chatService.ProcessMessage(query).GetAwaiter().GetResult();
        }
        
        return response;


    }
}