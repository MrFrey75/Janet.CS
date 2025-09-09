using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Janet.CLI.Models;
using Janet.CLI.Utilities.Logging;

namespace Janet.CLI.Services;

public class OllamaApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public OllamaApiService(ILogger logger, AppSettings settings)
    {
        _logger = logger;
        _httpClient = new HttpClient { BaseAddress = new Uri(settings.Ollama.BaseUrl) };
    }

    public async Task<List<OllamaModel>> GetAvailableModelsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.Log("Requesting /api/tags", ILogger.LogLevel.Debug);
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
            response.EnsureSuccessStatusCode();
            var modelsResponse = await response.Content.ReadFromJsonAsync<OllamaTagsResponse>(cancellationToken: cancellationToken);
            _logger.Log($"Found {modelsResponse?.Models?.Count ?? 0} models.", ILogger.LogLevel.Debug);
            return modelsResponse?.Models ?? [];
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Failed to connect to Ollama API. Please ensure it is running.", ex);
            return [];
        }
    }
    
    public async Task<IntentResponse> GetIntentAsync(string classifierModel, string userMessage, CancellationToken cancellationToken)
    {
        // System prompt for intent classification
        const string systemPrompt = @"You are an expert intent classification system..."; // Truncated for brevity
        try
        {
            var requestPayload = new { model = classifierModel, format = "json", messages = new[] { new { role = "system", content = systemPrompt }, new { role = "user", content = userMessage } }, stream = false };
            _logger.Log($"Requesting intent from model '{classifierModel}'.", ILogger.LogLevel.Debug);
            var response = await _httpClient.PostAsJsonAsync("/api/chat", requestPayload, cancellationToken);
            response.EnsureSuccessStatusCode();
            var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: cancellationToken);
            var intentJson = ollamaResponse?.Message?.Content ?? "{}";
            _logger.Log($"Raw JSON from classifier: {intentJson}", ILogger.LogLevel.Debug);
            return JsonSerializer.Deserialize<IntentResponse>(intentJson) ?? new IntentResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to get intent for message: '{userMessage}'", ex);
            return new IntentResponse { Intent = "general_chat", Confidence = 0.0 };
        }
    }

    public async Task<string> StreamChatAsync(string chatModel, List<ChatMessage> history, Action<string> onChunkReceived, CancellationToken cancellationToken)
    {
        var requestPayload = new { model = chatModel, messages = history, stream = true };
        var fullResponse = new StringBuilder();
        try
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/chat") { Content = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json") };
            _logger.Log($"Streaming chat from model '{chatModel}'.", ILogger.LogLevel.Debug);
            using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(responseStream);
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(line)) continue;
                var chatChunk = JsonSerializer.Deserialize<OllamaChatStreamResponse>(line);
                if (chatChunk?.Message?.Content != null)
                {
                    var content = chatChunk.Message.Content;
                    onChunkReceived(content);
                    fullResponse.Append(content);
                }
                if (chatChunk?.Done ?? false) break;
            }
        }
        catch (OperationCanceledException) { _logger.Log("Chat stream cancelled.", ILogger.LogLevel.Warning); }
        catch (Exception ex) { _logger.LogError($"Failed during streaming chat with model {chatModel}.", ex); onChunkReceived($"\n[red]An error occurred.[/]"); }
        return fullResponse.ToString();
    }
}