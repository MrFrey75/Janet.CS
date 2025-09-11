using Janet.DubCore.Models;
using Janet.DubCore.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Janet.DubCore.Functions;

public class ChatService
{
    private readonly ILoggingService _loggingService;
    private readonly IAppConfigService _appConfigService;
    private string _ollamaApiUrl;
    private string _modelName;
    private static readonly HttpClient client = new HttpClient();

    public ChatService(ILoggingService loggingService, IAppConfigService appConfigService)
    {
        _loggingService = loggingService;
        _appConfigService = appConfigService;

        // Ensure the URL from config points to the /api/generate endpoint
        var baseUrl = _appConfigService.Settings.Ollama.BaseUrl.TrimEnd('/');
        _ollamaApiUrl = $"{baseUrl}/api/generate";
        _modelName = _appConfigService.Settings.Ollama.ChatModelName;

        _loggingService.LogInfo("ChatService initialized.");
    }

    // Implement chat-related functionalities here

    /// <summary>
    /// Processes a chat message by logging it and returning an echo response.
    /// </summary>
    /// <param name="message">The chat message to process.</param>
    /// <returns>
    /// A <see cref="Task{ChatResponse}"/> representing the asynchronous operation, 
    /// containing the echo response with a timestamp.
    /// </returns>
    public async Task<ChatResponse> ProcessMessage(string message)
    {
        _loggingService.LogInfo($"Sending chat message: {message}");
        var systemPrompt = "You are a helpful assistant.";

                // The request payload for the Ollama API.
        var requestPayload = new OllamaRequest
        {
            Model = _modelName,
            System = systemPrompt,
            Prompt = $"User Query: \"{message}\"",
            Stream = false
        };

            ChatResponse chatResponse = new ChatResponse();

        try
        {
            string jsonPayload = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Send the POST request to the Ollama API.
            HttpResponseMessage response = await client.PostAsync(_ollamaApiUrl, content);

            // Read the response content regardless of status code.
            string responseBody = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                // The model's output is nested within the API response.
                var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseBody);
                if (ollamaResponse != null && !string.IsNullOrWhiteSpace(ollamaResponse.Response))
                {
                    chatResponse = new ChatResponse(ollamaResponse.Response)
                    {
                        Timestamp = DateTime.UtcNow
                    };
                    return chatResponse;
                }
                else
                {
                    _loggingService.LogWarning("Ollama response is null or empty.");
                    chatResponse = new ChatResponse("Model response is null or empty.")
                    {
                        Timestamp = DateTime.UtcNow
                    };
                    return chatResponse;
                }

            }
            else
            {
                _loggingService.LogError($"Ollama API call failed with status code: {(int)response.StatusCode}");
                _loggingService.LogDebug("Ollama error response:", responseBody);
                // Handle non-successful HTTP status codes.
                chatResponse = new ChatResponse("Failed to call Ollama API.")
                {
                    Timestamp = DateTime.UtcNow
                };
                return chatResponse;
            }
        }        catch (HttpRequestException e)
        {
            _loggingService.LogError("HTTP request to Ollama API failed: " + e.Message);
            _loggingService.LogDebug("Ollama error response:", e.ToString());
            // Handle network-related errors (e.g., Ollama server is not running).
            chatResponse = new ChatResponse("Could not connect to the Ollama service. Please ensure it is running.")
            {
                Timestamp = DateTime.UtcNow
            };
            return chatResponse;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Error processing chat message: " + ex.Message);
            throw;
        }

    }
}