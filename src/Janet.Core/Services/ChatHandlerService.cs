using System.Text.Json;
using Janet.Core.Enums;
using Janet.Core.Logging;
using Janet.Core.Models;
using Spectre.Console;

namespace Janet.Core.Services;

public interface IChatHandlerService
{
    IReadOnlyList<ChatMessage> ConversationHistory { get; }
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task<string> StreamResponseAsync(string userMessage, string chatModel, CancellationToken cancellationToken = default);
    Task ClearHistoryAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Manages a chat conversation, including history persistence and streaming 
/// interaction with the Ollama API.
/// </summary>
public class ChatHandlerService : IChatHandlerService
{
    private readonly ISpectreLogger _logger;
    private readonly OllamaApiService _apiService;
    private readonly ConfigService _configService;
    private static readonly string FilePath = Path.Combine("data", "history.json");

    private List<ChatMessage> _conversationHistory = new();
    public IReadOnlyList<ChatMessage> ConversationHistory => _conversationHistory.AsReadOnly();

public ChatHandlerService(ISpectreLogger logger, OllamaApiService apiService, ConfigService configService)
{
    _logger = logger;
    _apiService = apiService;
    // Correctly use the injected dependency
    _configService = configService; 
    _ = InitializeAsync();
}

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _conversationHistory = await LoadChatHistoryAsync(cancellationToken);
        _logger.Info($"Loaded {_conversationHistory.Count} messages from chat history.");
    }

    public async Task<string> StreamResponseAsync(string userMessage, string chatModel, CancellationToken cancellationToken = default)
    {
        // 1. Add user message to the internal history
        var userChatMessage = new ChatMessage(ChatMessageType.User, userMessage);
        _conversationHistory.Add(userChatMessage);

        // 2. Stream the API response
        AnsiConsole.Markup($"[bold yellow]{_configService.GetAiName()}:[/] ");
        var fullResponse = await _apiService.StreamChatAsync(chatModel, _conversationHistory, AnsiConsole.Write, cancellationToken);
        AnsiConsole.WriteLine();

        // 3. Handle the response
        if (!string.IsNullOrWhiteSpace(fullResponse))
        {
            _conversationHistory.Add(new ChatMessage(ChatMessageType.Assistant, fullResponse));
        }
        else
        {
            // If the AI gives no response, remove the user's message to allow a clean retry.
            _conversationHistory.Remove(userChatMessage);
            _logger.Warning("Assistant returned an empty response. Removed last user message.");
        }

        // 4. Save the final state of the history
        await SaveChatHistoryAsync(cancellationToken);
        return fullResponse;
    }

    public async Task ClearHistoryAsync(CancellationToken cancellationToken = default)
    {
        _conversationHistory.Clear();
        await SaveChatHistoryAsync(cancellationToken);
        _logger.Info("Chat history has been cleared.");
    }

    private async Task SaveChatHistoryAsync(CancellationToken cancellationToken)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? string.Empty);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_conversationHistory, options);
            await File.WriteAllTextAsync(FilePath, jsonString, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to save chat history", ex);
        }
    }

    private async Task<List<ChatMessage>> LoadChatHistoryAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(FilePath))
        {
            return new List<ChatMessage>();
        }
        try
        {
            string jsonString = await File.ReadAllTextAsync(FilePath, cancellationToken);
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return new List<ChatMessage>();
            }

            var history = JsonSerializer.Deserialize<List<ChatMessage>>(jsonString);
            return history ?? new List<ChatMessage>();
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to load or parse chat history at '{FilePath}'", ex);
            return new List<ChatMessage>(); // Return an empty list on failure
        }
    }
}