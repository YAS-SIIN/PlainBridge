using System.Net.WebSockets;

namespace PlainBridge.Server.Application.Management.WebSocket;

public interface IWebSocketManagement
{ 
    Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default!);
    Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken = default!);
    Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken = default!);
}