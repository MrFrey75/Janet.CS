// This class defines the structure of your config file.
// You can add any properties you need, including nested classes.
using Janet.DubCore.Enum;

namespace Janet.DubCore.Models;

public class AppSettings
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public PersonaDetails Persona { get; set; } = new PersonaDetails();
    public OllamaSettings Ollama { get; set; } = new OllamaSettings();
    public LoggingSettings Logging { get; set; } = new LoggingSettings();

    //public string SystemPrompt { get; } => $"You are {Persona.AssistantName}, a {Persona.Style} AI assistant. Your behavior is {Persona.Behavior}. Your goals are: {Persona.Goals}.";
}

public class OllamaSettings
{
    public string BaseUrl { get; set; } = "http://127.0.0.1:11434";
    public int MaxTokens { get; set; } = 2048;
    public double Temperature { get; set; } = 0.7;
    public string IntentModelName { get; set; } = "phi3";
    public string ChatModelName { get; set; } = "phi3";
    // Add other Ollama-specific settings here  


}

public class LoggingSettings
{
    public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    public bool LogToFile { get; set; } = false;
    public string LogFilePath { get; set; } = "logs/app.log";
    public bool LogToConsole { get; set; } = true;
    public bool LogToDebug { get; set; } = false;
    
}

public class PersonaDetails
{
    public string UserName { get; set; } = "User";
    public string AssistantName { get; set; } = "Assistant";
    public string Style { get; set; } = "Friendly";
    public string Behavior { get; set; } = "Helpful and informative";
    public string Goals { get; set; } = "Assist the user with their requests";
}