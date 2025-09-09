using System.Runtime.CompilerServices;
using Spectre.Console;

namespace Janet.CLI.Utilities.Logging;

/// <summary>
/// An implementation of ILogger that uses Spectre.Console for rich, colored output.
/// </summary>
public class SpectreLogger : ILogger
{
    private ILogger.LogLevel _currentLogLevel = ILogger.LogLevel.Debug;

    public void SetLogLevel(ILogger.LogLevel level)
    {
        _currentLogLevel = level;
    }

    public void Log(string message, ILogger.LogLevel level = ILogger.LogLevel.Info, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
    {
        if (level < _currentLogLevel) return;
        string source = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{memberName}";
        var prefix = level switch
        {
            ILogger.LogLevel.Debug => $"[grey]DEBUG | {source}[/]",
            ILogger.LogLevel.Info => $"[blue]INFO | {source}[/]",
            ILogger.LogLevel.Warning => $"[yellow]WARN | {source}[/]",
            ILogger.LogLevel.Error => $"[red]ERROR | {source}[/]",
            _ => ""
        };
        AnsiConsole.MarkupLine($"{prefix}: {message}");
    }

    public void LogError(string message, Exception ex, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
    {
        if (ILogger.LogLevel.Error < _currentLogLevel) return;
        string source = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{memberName}";
        AnsiConsole.MarkupLine($"[red]ERROR | {source}[/]: {message}");
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    }
}