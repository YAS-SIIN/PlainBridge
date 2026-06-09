using PlainBridge.Client.Application.Management.WebSocket;

namespace PlainBridge.Client.Application.Services.WebSocket;

public interface IWebSocketService
{
    Task<IWebSocketManagement> InitializeWebSocketAsync(string host, Uri internalUri, CancellationToken cancellationToken);
}