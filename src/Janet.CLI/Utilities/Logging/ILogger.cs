namespace Janet.CLI.Utilities.Logging;

/// <summary>
/// Defines the contract for a logging service.
/// </summary>
public interface ILogger
{
    enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    void SetLogLevel(LogLevel level);
    void Log(string message, LogLevel level = LogLevel.Info, [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string memberName = "");
    void LogError(string message, Exception ex, [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string memberName = "");
}