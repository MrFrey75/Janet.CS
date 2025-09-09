using System.Text.Json;
using Janet.CLI.Utilities.Logging;

namespace Janet.CLI.Services;

public class ConfigService
{
    private readonly OllamaApiService _apiService;
    private readonly ISpectreLogger _logger;

    private ConfigSettings _settings = new();
    private const string SettingsFile = "Data/settings.json";

    public ConfigService(OllamaApiService apiService, ISpectreLogger logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task LoadSettingsAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var stream = new FileStream(SettingsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var settings = await JsonSerializer.DeserializeAsync<ConfigSettings>(stream, cancellationToken: cancellationToken);
            if (settings != null)
            {
                // Apply settings to the application as needed
                _logger.Info("Settings loaded successfully.");
            }
            else
            {
                _logger.Warning("Settings file is empty or malformed.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to load settings from Ollama API.", ex);
            throw;
        }
    }

    public ConfigSettings GetSettings() => _settings;

    public void UpdateSetting(string key, object value)
    {
        var property = typeof(ConfigSettings).GetProperty(key);
        if (property != null && property.CanWrite)
        {
            property.SetValue(_settings, Convert.ChangeType(value, property.PropertyType));
            _logger.Info($"Setting '{key}' updated to '{value}'.");
        }
        else
        {
            _logger.Warning($"Setting '{key}' not found or is read-only.");
        }
        SaveSettingsAsync(CancellationToken.None).Wait();
    }
    
    public async Task SaveSettingsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            using var stream = new FileStream(SettingsFile, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, _settings, options, cancellationToken);
            _logger.Info("Settings saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to save settings to Ollama API.", ex);
            throw;
        }
    }
}

public class ConfigSettings
{

    public string ApiUrl { get; set; } = "http://localhost:11434";    
    public string Classification { get; set; } = "";
    public string Name { get; set; } = "";
}