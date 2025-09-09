using Janet.CLI.Actions;
using Janet.CLI.Models;
using Janet.CLI.Utilities.Logging;
using Spectre.Console;

namespace Janet.CLI.Services;

public class IntentHandlerService
{
    private readonly OllamaApiService _apiService;
    private readonly ISpectreLogger _logger;
    private readonly IChatService _chatService;

    public IntentHandlerService(OllamaApiService apiService, ISpectreLogger logger, IChatService chatService)
    {
        _apiService = apiService;
        _logger = logger;
        _chatService = chatService;
    }

    public async Task HandleIntentAsync(IntentResponse intent, string userInput, List<ChatMessage> history, string chatModel, CancellationToken cancellationToken)
    {
        _logger.Info($"Handling intent: '{intent.Intent}' with confidence {intent.Confidence:P0}");
        history.Add(new ChatMessage("user", userInput));
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
                await _chatService.HandleGeneralChatAsync(history, chatModel, cancellationToken);
                break;
        }

        // ENHANCEMENT: If a tool was used, call the LLM again to get a natural summary of the result.
        if (shouldSummarizeToolResult && !cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.MarkupLine("[grey]AI is summarizing the action...[/]");
            await _chatService.HandleGeneralChatAsync(history, chatModel, cancellationToken);
        }
    }



}