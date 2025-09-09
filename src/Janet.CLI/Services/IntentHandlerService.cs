using Janet.CLI.Models;
using Janet.CLI.Utilities.Logging;
using Spectre.Console;

namespace Janet.CLI.Services;

public class IntentHandlerService
{
    private readonly OllamaApiService _apiService;
    private readonly ISpectreLogger _logger;

    public IntentHandlerService(OllamaApiService apiService, ISpectreLogger logger)
    {
        _apiService = apiService;
        _logger = logger;
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
                SimulateWeatherTool(intent, history);
                shouldSummarizeToolResult = true;
                break;
            case "send_message":
                SimulateMessageTool(intent, history);
                shouldSummarizeToolResult = true;
                break;
            default:
                await HandleGeneralChatAsync(history, chatModel, cancellationToken);
                break;
        }

        // ENHANCEMENT: If a tool was used, call the LLM again to get a natural summary of the result.
        if (shouldSummarizeToolResult && !cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.MarkupLine("[grey]AI is summarizing the action...[/]");
            await HandleGeneralChatAsync(history, chatModel, cancellationToken);
        }
    }

    private void SimulateWeatherTool(IntentResponse intent, List<ChatMessage> history)
    {
        var city = intent.Entities?.GetValueOrDefault("city") ?? "an unspecified location";
        AnsiConsole.MarkupLine($"[bold green]Action:[/] Simulating call to weather API for [underline]{city}[/].");
        var toolResponse = $"The weather in {city} is 72°F and sunny.";
        // Use a "tool" role for system-level observations
        history.Add(new ChatMessage("tool", toolResponse));
    }

    private void SimulateMessageTool(IntentResponse intent, List<ChatMessage> history)
    {
        var recipient = intent.Entities?.GetValueOrDefault("recipient") ?? "unknown";
        var message = intent.Entities?.GetValueOrDefault("message") ?? "empty message";
        AnsiConsole.MarkupLine($"[bold green]Action:[/] Simulating sending message to [underline]{recipient}[/]: '{message}'");
        var toolResponse = $"Message successfully sent to {recipient}.";
        history.Add(new ChatMessage("tool", toolResponse));
    }

    private async Task HandleGeneralChatAsync(List<ChatMessage> history, string chatModel, CancellationToken cancellationToken)
    {
        AnsiConsole.Markup("[bold yellow]AI:[/] ");
        var fullResponse = await _apiService.StreamChatAsync(chatModel, history, AnsiConsole.Write, cancellationToken);
        AnsiConsole.WriteLine();

        if (!string.IsNullOrWhiteSpace(fullResponse))
        {
            history.Add(new ChatMessage("assistant", fullResponse));
        }
        else if (history.LastOrDefault()?.Role == "user")
        {
            history.RemoveAt(history.Count - 1);
            _logger.Warning("Assistant returned an empty response. Removed last user message to allow retry.");
        }
    }
}