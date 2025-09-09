using System.Runtime.CompilerServices;
using Spectre.Console;

namespace Janet.CLI.Utilities.Logging;

/// <summary>
/// A simple static logger class that uses Spectre.Console for colored output.
/// </summary>
public static class LoggerService
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// The current logging level. Messages below this level will not be displayed.
    /// </summary>
    private static LogLevel CurrentLogLevel { get; set; } = LogLevel.Debug; // Default to Debug as requested.

    /// <summary>
    /// Logs a standard message.
    /// </summary>
    public static void Log(string message, LogLevel level = LogLevel.Info, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
    {
        if (level < CurrentLogLevel)
        {
            return;
        }

        var source = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{memberName}";

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

    /// <summary>
    /// Logs an error with an associated exception.
    /// </summary>
    public static void LogError(string message, Exception ex, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
    {
        if (LogLevel.Error < CurrentLogLevel)
        {
            return;
        }
        
        var source = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{memberName}";

        AnsiConsole.MarkupLine($"[red]ERROR | {source}[/]: {message}");
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    }
}