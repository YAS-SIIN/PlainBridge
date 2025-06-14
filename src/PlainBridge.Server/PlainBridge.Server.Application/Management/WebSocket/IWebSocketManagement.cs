using System.Net.WebSockets;

namespace PlainBridge.Server.Application.Management.WebSocketManagement;

public interface IWebSocketManagement
{
    Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);
    Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);
}