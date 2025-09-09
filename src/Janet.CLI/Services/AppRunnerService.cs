using Janet.CLI.Models;
using Janet.CLI.Utilities.Logging;
using Spectre.Console;

namespace Janet.CLI.Services;

public class AppRunnerService
{
    private readonly OllamaApiService _apiService;
    private readonly IntentHandlerService _intentHandlerService;
    private readonly ISpectreLogger _logger;

    public AppRunnerService(
        OllamaApiService apiService,
        IntentHandlerService intentHandlerService,
        ISpectreLogger logger)
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
        _logger.Info("Application starting.");

        try
        {
            var availableModels = await GetAndVerifyModelsAsync(cancellationToken);
            if (availableModels.Count == 0) return;

            var classifierModel = SelectModel("Select a [yellow]fast[/] model for classification:", availableModels);
            var chatModel = SelectModel("Select a [green]powerful[/] model for chat:", availableModels);
            _logger.Info($"Using '{classifierModel}' for intent and '{chatModel}' for chat.");

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[grey]Classifier:[/] {classifierModel} | [grey]Chat Model:[/] {chatModel}");
            AnsiConsole.MarkupLine("Type a message. '[red]exit[/]' to quit.");

            var conversationHistory = new List<ChatMessage>();

            while (!cancellationToken.IsCancellationRequested)
            {
                string userInput;

                if (AnsiConsole.Profile.Capabilities.Interactive)
                {
                    userInput = AnsiConsole.Ask<string>("\n[bold blue]You:[/] ");
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Non-interactive mode: reading from Console.ReadLine()[/]");
                    userInput = Console.ReadLine() ?? string.Empty;
                }

                if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                // Intent classification
                AnsiConsole.MarkupLine("[grey]🧠 Classifying intent...[/]");
                var intent = await _apiService.GetIntentAsync(classifierModel, userInput, cancellationToken);

                // Streaming chat response
                AnsiConsole.MarkupLine("[grey]💬 Chatting...[/]");
                    await _intentHandlerService.HandleIntentAsync(intent, userInput, conversationHistory, chatModel, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            /* Expected on Ctrl+C */
        }
        catch (Exception ex)
        {
            _logger.Error("An unexpected, fatal error occurred.", ex);
        }
        finally
        {
            AnsiConsole.MarkupLine("\n[yellow]Exiting application.[/]");
            _logger.Info("Application shutting down.");
        }
    }

    private async Task<List<OllamaModel>> GetAndVerifyModelsAsync(CancellationToken cancellationToken)
    {
        List<OllamaModel> models = new();
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
        if (AnsiConsole.Profile.Capabilities.Interactive)
        {
            var prompt = new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .AddChoices(models.Select(m => m.Name));
            return AnsiConsole.Prompt(prompt);
        }

        // Non-interactive fallback
        var fallback = models.First().Name;
        AnsiConsole.MarkupLine($"[yellow]Non-interactive environment detected. Defaulting to:[/] {fallback}");
        return fallback;
    }
}
