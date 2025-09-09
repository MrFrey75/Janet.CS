using Janet.CLI.Models;

namespace Janet.CLI.Services;

/// <summary>
/// Defines the contract for a service that manages a chat conversation,
/// including history persistence and interaction with an AI model.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Gets the current conversation history in a read-only state.
    /// </summary>
    IReadOnlyList<ChatMessage> ConversationHistory { get; }

    /// <summary>
    /// Asynchronously loads the chat history from persistence. This must be 
    /// called before any other methods.
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a user message, streams the AI's response, and updates the history.
    /// </summary>
    /// <returns>The complete response from the assistant as a string.</returns>
    Task<string> StreamResponseAsync(string userMessage, string chatModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the current conversation history from memory and storage.
    /// </summary>
    Task ClearHistoryAsync(CancellationToken cancellationToken = default);
}