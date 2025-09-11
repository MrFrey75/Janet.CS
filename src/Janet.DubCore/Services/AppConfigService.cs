using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Janet.DubCore.Models;
using Janet.DubCore.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class AppConfigService : IAppConfigService
{
    private readonly string _filePath;
    private AppConfigSettings _settings;
    // Use a SemaphoreSlim to ensure thread-safe file access
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AppConfigSettings Settings => _settings;

    public AppConfigService()
    {
        _settings = new AppConfigSettings();
        _filePath = Path.Combine(AppContext.BaseDirectory, "data", "appsettings.yaml");
        // Ensure the directory exists
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!File.Exists(_filePath))
        {
            SaveAsync().GetAwaiter().GetResult(); // Save it to create the initial fileAppConfigSettings
            return;
        }

        // Note: Blocking on async code like this can be problematic in some contexts
        // (e.g., UI threads). For DI in console/web apps, it's often acceptable.
        // A better pattern is an async factory, but this is simpler for this example.
        LoadAsync().GetAwaiter().GetResult();
    }
    
    public async Task LoadAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
            {
                _settings = new AppConfigSettings(); // Create instance with default values
                await SaveAsync();     // Save it to create the initial file
            }

            var yaml = await File.ReadAllTextAsync(_filePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            _settings = deserializer.Deserialize<AppConfigSettings>(yaml) ?? new AppConfigSettings();
        }
        finally
        {
            _semaphore.Release();
        }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await SaveInternalAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task UpdateAsync(Action<AppConfigSettings> updateAction)
    {
        await _semaphore.WaitAsync();
        try
        {
            // Perform the update on the in-memory settings object
            updateAction(_settings);
            
            // Persist the changes to the file
            await SaveInternalAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Internal save method that doesn't acquire the semaphore, to be used by public methods.
    /// </summary>
    private async Task SaveInternalAsync()
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(_settings);
        await File.WriteAllTextAsync(_filePath, yaml);
    }
}