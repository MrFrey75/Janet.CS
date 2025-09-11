using System;
using Janet.DubCore.Enum;

namespace Janet.DubCore.Services;

/// <summary>
/// Defines the contract for a logging service.
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    void LogDebug(string message);
    
    /// <summary>
    /// Logs a debug message with associated data.
    /// </summary>
    void LogDebug(string message, object data);

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    void LogInfo(string message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void LogWarning(string message);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    void LogError(string message);

    /// <summary>
    /// Logs an error message with an associated exception.
    /// </summary>
    void LogError(string message, Exception ex);
    
    /// <summary>
    /// Logs an error message with associated data and an exception.
    /// </summary>
    void LogError(string message, object data, Exception ex);
}