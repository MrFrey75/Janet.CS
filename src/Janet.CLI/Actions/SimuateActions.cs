using Janet.CLI.Models;
using Spectre.Console;

namespace Janet.CLI.Actions;

public static class SimulateActions
{
    public static void SimulateMessageTool(IntentResponse intent, List<ChatMessage> history)
    {
        var recipient = intent.Entities?.GetValueOrDefault("recipient") ?? "unknown";
        var message = intent.Entities?.GetValueOrDefault("message") ?? "empty message";
        AnsiConsole.MarkupLine($"[bold green]Action:[/] Simulating sending message to [underline]{recipient}[/]: '{message}'");
        var toolResponse = $"Message successfully sent to {recipient}.";
        history.Add(new ChatMessage("tool", toolResponse));
    }

    public static void SimulateWeatherTool(IntentResponse intent, List<ChatMessage> history)
    {
        var city = intent.Entities?.GetValueOrDefault("city") ?? "an unspecified location";
        AnsiConsole.MarkupLine($"[bold green]Action:[/] Simulating call to weather API for [underline]{city}[/].");
        var toolResponse = $"The weather in {city} is 72°F and sunny.";
        // Use a "tool" role for system-level observations
        history.Add(new ChatMessage("tool", toolResponse));
    }

}