using PlainBridge.Server.Application.Management.WebSocketManagement;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Services.WebSocket
{
    public interface IWebSocketService
    {
        Task HandleWebSocketAsync(IWebSocketManagement webSocketManagement, HostApplicationDto hostApplication, CancellationToken cancellationToken);
        Task InitializeConsumerAsync(CancellationToken cancellationToken);
    }
}