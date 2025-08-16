using PlainBridge.Client.Application.Management.WebSocket; 
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Client.Application.Management.Cache;

public interface ICacheManagement
{
    Task<HostApplicationDto> SetGetHostApplicationAsync(string host, HostApplicationDto value = default!, CancellationToken cancellationToken = default!);
    Task<ServerApplicationDto> SetGetServerApplicationAsync(Guid appId, ServerApplicationDto value = default!, CancellationToken cancellationToken = default!);
    Task<ServerApplicationDto> SetGetServerApplicationAsync(string username, int port, ServerApplicationDto value = default!, CancellationToken cancellationToken = default!);
    Task<IWebSocketManagement> SetGetWebSocketAsync(string host, IWebSocketManagement value = default!, CancellationToken cancellationToken = default!);
    Task RemoveWebSocketAsync(string host, CancellationToken cancellationToken = default!);
}