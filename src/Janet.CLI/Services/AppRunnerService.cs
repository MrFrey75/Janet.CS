using Janet.CLI.Models;
using Janet.CLI.Utilities.Logging;
using Spectre.Console;

namespace Janet.CLI.Services;

public class AppRunnerService
{
    private readonly OllamaApiService _apiService;
    private readonly IntentHandlerService _intentHandlerService;
    private readonly ILogger _logger;

    public AppRunnerService(OllamaApiService apiService, IntentHandlerService intentHandlerService, ILogger logger)
    {
        _apiService = apiService;
        _intentHandlerService = intentHandlerService;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.Title = "Ollama Agent";
        AnsiConsole.Write(new FigletText("Ollama Agent").Centered().Color(Color.Orange1));
        AnsiConsole.MarkupLine("[grey]An intelligent chat client that classifies intent before responding.[/]");
        _logger.Log("Application starting.", ILogger.LogLevel.Info);
        
        try
        {
            var availableModels = await GetAndVerifyModelsAsync(cancellationToken);
            if (availableModels.Count == 0) return;

            var classifierModel = SelectModel("Select a [yellow]fast[/] model for classification:", availableModels);
            var chatModel = SelectModel("Select a [green]powerful[/] model for chat:", availableModels);
            _logger.Log($"Using '{classifierModel}' for intent and '{chatModel}' for chat.");

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[grey]Classifier:[/] {classifierModel} | [grey]Chat Model:[/] {chatModel}");
            AnsiConsole.MarkupLine("Type a message. '[red]exit[/]' to quit.");
            
            var conversationHistory = new List<ChatMessage>();
            while (!cancellationToken.IsCancellationRequested)
            {
                var userInput = AnsiConsole.Ask<string>("\n[bold blue]You:[/] ");
                if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                await AnsiConsole.Status().Spinner(Spinner.Known.Dots).StartAsync("🧠 Thinking...", async ctx =>
                {
                    var intent = await _apiService.GetIntentAsync(classifierModel, userInput, cancellationToken);
                    await _intentHandlerService.HandleIntentAsync(intent, userInput, conversationHistory, chatModel, cancellationToken);
                });
            }
        }
        catch (OperationCanceledException) { /* Expected on Ctrl+C */ }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected, fatal error occurred.", ex);
        }
        finally
        {
            AnsiConsole.MarkupLine("\n[yellow]Exiting application.[/]");
            _logger.Log("Application shutting down.", ILogger.LogLevel.Info);
        }
    }
    
    private async Task<List<OllamaModel>> GetAndVerifyModelsAsync(CancellationToken cancellationToken)
    {
        List<OllamaModel> models = [];
        await AnsiConsole.Status().StartAsync("Connecting to Ollama...", async ctx =>
        {
            models = await _apiService.GetAvailableModelsAsync(cancellationToken);
        });
        if (models.Count != 0) return models;
        AnsiConsole.MarkupLine("[red]Error:[/] Could not connect to Ollama or no models were found.");
        AnsiConsole.MarkupLine("[grey]Please ensure Ollama is running and has models available.[/]");
        return models;
    }

    private static string SelectModel(string title, List<OllamaModel> models)
    {
        var prompt = new SelectionPrompt<string>().Title(title).PageSize(10).AddChoices(models.Select(m => m.Name));
        return AnsiConsole.Prompt(prompt);
    }
}