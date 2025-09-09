using Spectre.Console;
using System.IO;
using System.Runtime.CompilerServices;

namespace Janet.CLI.Utilities.Logging;

/// <summary>
/// An implementation of ILogger that uses Spectre.Console for rich, colored output.
/// </summary>
public class SpectreLogger : ISpectreLogger
{
    private LogLevel _currentLogLevel = LogLevel.Info; // Changed default to Info

    public void SetLogLevel(LogLevel level)
    {
        _currentLogLevel = level;
    }

    public LogLevel GetLogLevel() => _currentLogLevel;

    private void Log(string message, LogLevel level, string sourceFilePath, string memberName)
    {
        if (level < _currentLogLevel) return;

        string source = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{memberName}";
        var prefix = level switch
        {
            LogLevel.Debug   => new Markup($"[grey]DEBUG | {source}[/]: "),
            LogLevel.Info    => new Markup($"[blue]INFO | {source}[/]: "),
            LogLevel.Warning => new Markup($"[yellow]WARN | {source}[/]: "),
            LogLevel.Error   => new Markup($"[red]ERROR | {source}[/]: "),
            LogLevel.Message   => new Markup($"[blue]MESSAGE | {source}[/]: "),
            _ => new Markup($"[grey]UNKNOWN | {source}[/]: ")
        };

        AnsiConsole.Write(prefix);
        AnsiConsole.MarkupLine(message); // Log the message itself
    }

    public void Message(string message)
    {
        Log(message, LogLevel.Message, "", "");
    }

    public void Debug(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        => Log(message, LogLevel.Debug, sourceFilePath, memberName);

    public void Info(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        => Log(message, LogLevel.Info, sourceFilePath, memberName);

    public void Warning(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        => Log(message, LogLevel.Warning, sourceFilePath, memberName);

    // Implementation for the simple error message overload
    public void Error(string message, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
        => Log(message, LogLevel.Error, sourceFilePath, memberName);

    // Corrected and improved implementation for the exception overload
    public void Error(string message, Exception ex, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string memberName = "")
    {
        if (LogLevel.Error < _currentLogLevel) return;

        string source = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{memberName}";
        var prefix = new Markup($"[red]ERROR | {source}[/]: ");

        AnsiConsole.Write(prefix);
        AnsiConsole.MarkupLine(message); // Log the user-provided message first

        // Use Spectre.Console's rich exception rendering
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
    }
}

