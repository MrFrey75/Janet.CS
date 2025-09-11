
namespace Janet.DubCore.Models;

public class OllamaIntent
{
    public string intent { get; set; } = string.Empty;
    public Dictionary<string, string> entities { get; set; } = new Dictionary<string, string>();
}