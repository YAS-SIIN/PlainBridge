using System.Net.Sockets;
using PlainBridge.Client.Application.DTOs;
using PlainBridge.Client.Application.Management.WebSocket; 
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Client.Application.Management.Cache;

public interface ICacheManagement
{
    Task<HostApplicationDto> SetGetHostApplicationAsync(string host, HostApplicationDto value = default!, CancellationToken cancellationToken = default!);
    Task<ServerApplicationDto> SetGetServerApplicationAsync(string appId, ServerApplicationDto value = default!, CancellationToken cancellationToken = default!);
    Task<ServerApplicationDto> SetGetServerApplicationAsync(string username, int port, ServerApplicationDto value = default!, CancellationToken cancellationToken = default!);
    Task<List<ServerApplicationDto>> SetGetServerApplicationsAsync(List<ServerApplicationDto> value = default!, CancellationToken cancellationToken = default!);
    Task<IWebSocketManagement> SetGetWebSocketAsync(string host, IWebSocketManagement value = default!, CancellationToken cancellationToken = default!);
    Task RemoveWebSocketAsync(string host, CancellationToken cancellationToken = default!);
    Task<TcpListener> SetGetTcpListenerAsync(int port, TcpListener value = default!, CancellationToken cancellationToken = default);
    Task<UsePortCacheDto> SetGetUsePortModelAsync(int port, string connectionId, UsePortCacheDto value = default!, CancellationToken cancellationToken = default);
    Task<SharePortCacheDto> SetSharePortModelAsync(string useportUsername, int useportPort, string connectionId, SharePortCacheDto value = default!, CancellationToken cancellationToken = default);
}