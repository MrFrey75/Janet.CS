
namespace Janet.DubCore.Services;

public class Orchestrator
{
    private readonly IAppConfigService _configService;
    private readonly ILoggingService _loggingService;
    private readonly IntentService _intentService;

    public Orchestrator(IAppConfigService configService, ILoggingService loggingService, IntentService intentService)
    {
        _configService = configService;
        _loggingService = loggingService;
        _intentService = intentService;

        _loggingService.LogInfo("Orchestrator initialized.");
    }

    public string ProcessQuery(string query)
    {
        var intentJson = _intentService.GetIntentAndEntitiesAsJsonAsync(query).GetAwaiter().GetResult();
        // Implement your query processing logic here
        _loggingService.LogInfo($"Processing query: {query}");
        _loggingService.LogDebug("Extracted Intent and Entities:", System.Text.Json.JsonSerializer.Serialize(intentJson));
        return $"Processed query: {query}";
    }
}