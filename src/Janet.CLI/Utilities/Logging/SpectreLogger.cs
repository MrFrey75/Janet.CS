using System.Runtime.CompilerServices;
using Spectre.Console;

namespace Janet.CLI.Utilities.Logging;

/// <summary>
/// An implementation of ILogger that uses Spectre.Console for rich, colored output.
/// </summary>
public class SpectreLogger : ISpectreLogger
{
    private LogLevel _currentLogLevel = LogLevel.Debug;

    public void SetLogLevel(LogLevel level)
    {
        _currentLogLevel = level;
    }

    public LogLevel GetLogLevel() => _currentLogLevel;

    private void Log(string message, LogLevel level = LogLevel.Info, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
    {
        if (level < _currentLogLevel) return;
        string source = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{memberName}";
        var prefix = level switch
        {
            LogLevel.Debug => $"[grey]DEBUG | {source}[/]",
            LogLevel.Info => $"[blue]INFO | {source}[/]",
            LogLevel.Warning => $"[yellow]WARN | {source}[/]",
            LogLevel.Error => $"[red]ERROR | {source}[/]",
            _ => ""
        };
        AnsiConsole.MarkupLine($"{prefix}: {message}");
    }

    public void Debug(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        => Log(message, LogLevel.Debug, sourceFilePath, memberName);
    public void Info(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        => Log(message, LogLevel.Info, sourceFilePath, memberName);
    public void Warning(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        => Log(message, LogLevel.Warning, sourceFilePath, memberName);
    public void Error(string message, Exception ex, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        => Log(message + $": {ex.Message}", LogLevel.Error, sourceFilePath, memberName);
}