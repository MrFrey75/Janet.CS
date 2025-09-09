using System.Text.Json;
using Janet.CLI.Utilities.Logging;

namespace Janet.CLI.Services;

public class ConfigService
{
    private readonly OllamaApiService _apiService;
    private readonly ILogger _logger;
    
    public ConfigService(OllamaApiService apiService, ILogger logger)
    {
        _apiService = apiService;
        _logger = logger;
    }
    
    public async Task LoadSettingsAsync(CancellationToken cancellationToken)
    {
        var settingsFile = "Janet.CLI/Data/settings.json";
        try
        {
            using var stream = new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var settings = await JsonSerializer.DeserializeAsync<ConfigSettings>(stream, cancellationToken: cancellationToken);
            if (settings != null)
            {
                // Apply settings to the application as needed
                _logger.Log("Settings loaded successfully.", ILogger.LogLevel.Info);
            } else
            {
                _logger.LogError("Settings file is empty or malformed.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load settings from Ollama API.", ex);
            throw;
        }
    }
}

public class ConfigSettings
{
    public class OllamaSettings
    {
        public string ApiUrl { get; set; } = "http://localhost:11434";
    }
    
    public class OllamaModel
    {
        public string Classification { get; set; } = "";
        public string Name { get; set; } = "";
    }
}