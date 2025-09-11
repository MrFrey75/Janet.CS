
using Janet.DubCore.Functions;
using Janet.DubCore.Models;

namespace Janet.DubCore.Services;

public class Orchestrator
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
        var intentJson = _intentService.GetIntentAndEntitiesAsJsonAsync(query).GetAwaiter().GetResult();
        // Implement your query processing logic here

        if (intentJson.intent == "error")
        {
            _loggingService.LogWarning($"Error processing query: {query}.");
            return new ChatResponse($"Error processing query: {query}.");
        }

        switch(intentJson.intent.ToLower())
        {
            case "set_timer":
                if (intentJson.entities.TryGetValue("duration", out var duration))
                {
                    return new ChatResponse($"Timer set for {duration}.");
                }
                else
                {
                    return new ChatResponse("Error: 'duration' entity is missing for 'set_timer' intent.");
                }

            case "find_weather":
                if (intentJson.entities.TryGetValue("location", out var location))
                {
                    return new ChatResponse($"Fetching weather for {location}...");
                }
                else
                {
                    return new ChatResponse("Error: 'location' entity is missing for 'find_weather' intent.");
                }

            default:
                _loggingService.LogWarning($"Unknown intent '{intentJson.intent}' received.");
                return _chatService.ProcessMessage(query).GetAwaiter().GetResult();
        }


    }
}