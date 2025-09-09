using Janet.Core.Models;

namespace Janet.Core.Models;

public class AppSettings
{
    public OllamaSettings Ollama { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
}