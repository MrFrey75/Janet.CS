
using Janet.Core.Actions;
using Janet.Core.Enums;
using Janet.Core.Logging;
using Janet.Core.Models;
using Spectre.Console;

namespace Janet.Core.Services;

public class IntentHandlerService
{
    private readonly OllamaApiService _apiService;
    private readonly ISpectreLogger _logger;
    private readonly IChatHandlerService _ChatHandlerService;

    public IntentHandlerService(OllamaApiService apiService, ISpectreLogger logger, IChatHandlerService ChatHandlerService)
    {
        _apiService = apiService;
        _logger = logger;
        _ChatHandlerService = ChatHandlerService;
    }

    public async Task HandleIntentAsync(IntentResponse intent, string userInput, List<ChatMessage> history, string chatModel, CancellationToken cancellationToken)
    {
        _logger.Info($"Handling intent: '{intent.Intent}' with confidence {intent.Confidence:P0}");
        history.Add(new ChatMessage(ChatMessageType.User, userInput));
        bool shouldSummarizeToolResult = false;

        if (intent.Confidence < 0.7 && intent.Intent != "general_chat")
        {
            _logger.Warning("Confidence is low, switching to 'general_chat'.");
            intent = intent with { Intent = "general_chat" };
        }

        switch (intent.Intent)
        {
            case "get_weather":
                SimulateActions.SimulateWeatherTool(intent, history);
                shouldSummarizeToolResult = true;
                break;
            case "send_message":
                SimulateActions.SimulateMessageTool(intent, history);
                shouldSummarizeToolResult = true;
                break;
            default:
                await _ChatHandlerService.StreamResponseAsync(userInput, chatModel, cancellationToken);
                break;
        }

        // ENHANCEMENT: If a tool was used, call the LLM again to get a natural summary of the result.
        if (shouldSummarizeToolResult && !cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.MarkupLine("[grey]AI is summarizing the action...[/]");
        }
    }



}