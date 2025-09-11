using Janet.DubCore.Models;

namespace Janet.DubCore.Services;

public interface IOrchestrator
{
    ChatResponse ProcessQuery(string query);
}
