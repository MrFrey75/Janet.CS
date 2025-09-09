using System.Runtime.CompilerServices;

namespace Janet.CLI.Utilities.Logging;

/// <summary>
/// Defines the contract for a logger that uses Spectre.Console for output.
/// </summary>
public interface ISpectreLogger
{
    void SetLogLevel(LogLevel level);
    LogLevel GetLogLevel();

    void Debug(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "");
    void Info(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "");
    void Warning(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "");

    /// <summary>
    /// Logs an error message and a full exception.
    /// </summary>
    void Error(string message, Exception ex, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "");

    /// <summary>
    /// Logs a simple error message string.
    /// </summary>
    void Error(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "");
    void Message(string message);
}