namespace Janet.CLI.Utilities.Logging;

/// <summary>
/// Defines a contract for logging messages with different severity levels,
/// including support for configurable log levels.
/// </summary>
public interface ISpectreLogger
{
    /// <summary>
    /// Sets the minimum log level. Messages below this level will not be logged.
    /// </summary>
    void SetLogLevel(LogLevel level);

    /// <summary>
    /// Gets the current minimum log level.
    /// </summary>
    LogLevel GetLogLevel();

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    void Debug(string message, string sourceFilePath = "", string memberName = "");

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    void Info(string message, string sourceFilePath = "", string memberName = "");

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void Warning(string message, string sourceFilePath = "", string memberName = "");

    /// <summary>
    /// Logs an error message.
    /// </summary>
    void Error(string message, Exception ex, string sourceFilePath = "", string memberName = "");
}
