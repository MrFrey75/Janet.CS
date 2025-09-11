using Janet.DubCore.Services;

namespace Janet.DubCli.Services;

public class AppRunnerService
{
    private readonly IAppConfigService _configService;
    private readonly Orchestrator _orchestrator;
    private readonly ILoggingService _loggingService;

    public AppRunnerService(IAppConfigService configService, Orchestrator orchestrator, ILoggingService loggingService)
    {
        _configService = configService;
        _orchestrator = orchestrator;
        _loggingService = loggingService;
        _loggingService.LogInfo("AppRunnerService initialized.");
    }

    /// <summary>
    /// Runs the main application loop, displaying initial settings and handling user queries interactively.
    /// The method prints application settings, prompts the user for input, processes queries using the orchestrator,
    /// and logs exit information when the user types "exit".
    /// </summary>
    public void Run()
    {
        var settings = _configService.Settings;

        Console.WriteLine("AppRunnerService is running with the following settings:");
        Console.WriteLine($"CreatedAt: {settings.CreatedAt}");
        Console.WriteLine($"UpdatedAt: {settings.UpdatedAt}");

        Console.WriteLine("Press Ctrl+C to exit.");

        var query = string.Empty;
        while (query != "exit")
        {
            // Prompt user for input
            Console.Write($"{_configService.Settings.Persona.UserName}> ");
            query = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

            if (query == "exit")
            {
                _loggingService.LogInfo("Exiting application.");
                break;
            }
            else if (string.IsNullOrWhiteSpace(query))
            {
                continue; // Skip empty input
            }
            else if (query == "settings")
            {
                Console.WriteLine("Current Settings:");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented));
            }
            else if (!string.IsNullOrEmpty(query))
            {
                var response = _orchestrator.ProcessQuery(query);

                Console.WriteLine($"{_configService.Settings.Persona.AssistantName}> {response.Timestamp}: {response.Response}");
            }



        }   
    }
}