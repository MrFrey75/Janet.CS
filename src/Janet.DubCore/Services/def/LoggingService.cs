using System.Diagnostics;
using Janet.DubCore.Enum;
using Janet.DubCore.Models;

namespace Janet.DubCore.Services;

public class LoggingService : ILoggingService
{
    private readonly IAppConfigService _configService;

    public LoggingService(IAppConfigService configService)
    {
        _configService = configService;
    }
    
    private AppConfigSettings Settings => _configService.Settings;
    private string LogFilePath => Settings.Logging.LogFilePath;
    private LogLevel LogLevel => Settings.Logging.LogLevel;
    private bool LogToFile => Settings.Logging.LogToFile;
    private bool LogToConsole => Settings.Logging.LogToConsole;
    private bool LogToDebug => Settings.Logging.LogToDebug;


    private void Log(LogLevel level, string message)
    {
        if (level < LogLevel)
            return;

        string logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

        if (LogToConsole)
        {
            Console.WriteLine(logEntry);
        }

        if (LogToDebug)
        {
            Debug.WriteLine(logEntry);
        }

        if (LogToFile)
        {
            try
            {
                string logDir = Path.GetDirectoryName(LogFilePath) ?? string.Empty;
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log to file: {ex.Message}");
            }
        }
    }


    public void LogDebug(string message) => Log(LogLevel.Debug, message);
    public void LogDebug(string message, object data) => Log(LogLevel.Debug, $"{message}: {data}");
    public void LogInfo(string message) => Log(LogLevel.Info, message);
    public void LogWarning(string message) => Log(LogLevel.Warning, message);
    public void LogError(string message) => Log(LogLevel.Error, message);   
    public void LogError(string message, Exception ex) => Log(LogLevel.Error, $"{message}: {ex.Message}");
    public void LogError(string message, object data, Exception ex) => Log(LogLevel.Error, $"{message}: {data} - Exception: {ex.Message}");
}