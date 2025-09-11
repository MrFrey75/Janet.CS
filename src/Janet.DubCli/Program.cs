
using Janet.DubCli.Services;
using Janet.DubCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Janet.DubCli;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Janet.DubCli is running...");


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



        // var app = serviceProvider.GetRequiredService<AppRunnerService>();
        // await app.RunAsync(cancellationTokenSource.Token);

        var configService = serviceProvider.GetRequiredService<IAppConfigService>();
        configService.LoadAsync().GetAwaiter().GetResult();

        var settings = configService.Settings;
        var loggingService = serviceProvider.GetRequiredService<ILoggingService>();
        loggingService.LogInfo("Application started.");
        loggingService.LogDebug("Current Settings:", JsonConvert.SerializeObject(settings, Formatting.Indented));

        var appRunner = serviceProvider.GetRequiredService<AppRunnerService>();
        appRunner.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // 1. Build configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();


        // 5. Register ConfigService and load settingshi
        services.AddSingleton<IAppConfigService, AppConfigService>();
        services.AddSingleton<ILoggingService, LoggingService>();
        services.AddSingleton<IntentService>();
        services.AddSingleton<Orchestrator>();
        services.AddSingleton<AppRunnerService>();
    }
}
