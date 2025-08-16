namespace PlainBridge.Client.Application.Handler.WebSocket;

public interface IWebSocketHandler
{
    Task InitializeWebSocketConsumer(string username, CancellationToken cancellationToken);
}