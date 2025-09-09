using Janet.CLI.Models;
using Janet.CLI.Utilities.Logging;
using Spectre.Console;

namespace Janet.CLI.Services;

public interface IChatService
{
    Task HandleGeneralChatAsync(List<ChatMessage> history, string chatModel, CancellationToken cancellationToken);
}

public class ChatService : IChatService
{
    private readonly ISpectreLogger _logger;
    private readonly OllamaApiService _apiService;

    public ChatService(ISpectreLogger logger, OllamaApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task HandleGeneralChatAsync(List<ChatMessage> history, string chatModel, CancellationToken cancellationToken)
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