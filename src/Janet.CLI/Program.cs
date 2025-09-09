using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Janet.Core.Models;
using Janet.Core.Services;
using Janet.Core.Logging;
using Janet.CLI.Services;

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
        services.AddSingleton<ISpectreLogger, SpectreLogger>();

        // 4. Register application services
        services.AddTransient<OllamaApiService>();
        services.AddTransient<IntentHandlerService>();
        services.AddTransient<AppRunnerService>();

        // 5. Register ConfigService and load settings
        services.AddTransient<ConfigService>();
        var serviceProvider = services.BuildServiceProvider();
        var configService = serviceProvider.GetRequiredService<ConfigService>();
        configService.LoadSettingsAsync(CancellationToken.None).GetAwaiter().GetResult();

        // 6. Register ChatHandlerService
        services.AddSingleton<IChatHandlerService, ChatHandlerService>();

    }
}