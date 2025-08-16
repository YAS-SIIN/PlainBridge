namespace PlainBridge.Client.Application.Handler.WebSocket;

public interface IWebSocketHandler
{
    Task InitializeWebSocketConsumerAsync(string username, CancellationToken cancellationToken);
}