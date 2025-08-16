
namespace PlainBridge.Client.Application.Handler;

public interface IWebSocketHandler
{
    Task InitializeWebSocketConsumer(string username, CancellationToken cancellationToken);
}