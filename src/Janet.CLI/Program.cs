using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Janet.CLI.Models;
using Janet.CLI.Services;
using Janet.CLI.Utilities.Logging;

namespace Janet.CLI;

public static class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        // Handle graceful shutdown on Ctrl+C
        var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        var app = serviceProvider.GetRequiredService<AppRunnerService>();
        await app.RunAsync(cancellationTokenSource.Token);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // 1. Build configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        // 2. Bind configuration to a strongly-typed object and register it
        var appSettings = new AppSettings();
        configuration.Bind(appSettings);
        services.AddSingleton(appSettings);

        // 3. Register the logger
        services.AddSingleton<ILogger, SpectreLogger>();

        // 4. Register application services
        services.AddTransient<OllamaApiService>();
        services.AddTransient<IntentHandlerService>();
        services.AddTransient<AppRunnerService>();
    }
}