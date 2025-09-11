using Janet.DubCore.Models;

namespace Janet.DubCore.Services;

public interface IIntentService
{
    Task<ChatMessage> ProcessMessage(ChatMessage message);
}