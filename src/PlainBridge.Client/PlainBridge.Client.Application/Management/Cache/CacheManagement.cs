
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Client.Application.DTOs;
using PlainBridge.Client.Application.Management.WebSocket;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Client.Application.Management.Cache;

public class CacheManagement(ILogger<CacheManagement> _logger, HybridCache _memoryCache, IOptions<ApplicationSettings> _appSettings) : ICacheManagement
{
    HybridCacheEntryOptions options = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.Parse(_appSettings.Value.HybridDistributedCacheExpirationTime),
        LocalCacheExpiration = TimeSpan.Parse(_appSettings.Value.HybridMemoryCacheExpirationTime),

    };

    public async Task<HostApplicationDto> SetGetHostApplicationAsync(string host, HostApplicationDto value = default!, CancellationToken cancellationToken = default) => await _memoryCache.GetOrCreateAsync<HostApplicationDto>(
            $"hostApplication:{host}",
            async ct => value,
            options,
            cancellationToken: cancellationToken);

    public async Task<ServerApplicationDto> SetGetServerApplicationAsync(Guid appId, ServerApplicationDto value = default!, CancellationToken cancellationToken = default!) => await
        _memoryCache.GetOrCreateAsync($"serverApplication:appId:{appId}",
            async ct => value,
            options,
            cancellationToken: cancellationToken);

    public async Task<ServerApplicationDto> SetGetServerApplicationAsync(string username, int port, ServerApplicationDto value = default!, CancellationToken cancellationToken = default!) => await
        _memoryCache.GetOrCreateAsync($"serverApplication:username:{username}:port:{port}",
            async ct => value,
            options,
            cancellationToken: cancellationToken);

    public async Task<List<ServerApplicationDto>> SetGetServerApplicationsAsync(List<ServerApplicationDto> value = default!, CancellationToken cancellationToken = default) => await
        _memoryCache.GetOrCreateAsync($"serverApplications",
            async ct => value,
            options,
            cancellationToken: cancellationToken);

    public async Task<IWebSocketManagement> SetGetWebSocketAsync(string host, IWebSocketManagement value = default!, CancellationToken cancellationToken = default!) => await
        _memoryCache.GetOrCreateAsync($"webSocket:{host}",
            async ct => value,
            options,
            cancellationToken: cancellationToken);

    public async Task RemoveWebSocketAsync(string host, CancellationToken cancellationToken = default!) => await _memoryCache.RemoveAsync($"webSocket:{host}", cancellationToken);

     
    public async Task<TcpListener> SetGetTcpListenerAsync(int port, TcpListener value = default!, CancellationToken cancellationToken = default!) => await
        _memoryCache.GetOrCreateAsync($"{port}",
            async ct => value,
            cancellationToken: cancellationToken);


    public async Task<UsePortCacheDto> SetGetUsePortModelAsync(int port, string connectionId, UsePortCacheDto value = default!, CancellationToken cancellationToken = default!) => await
        _memoryCache.GetOrCreateAsync($"{port}_{connectionId}",
            async ct => value,
            cancellationToken: cancellationToken);

    public async Task<SharePortCacheDto> SetSharePortModelAsync(string useportUsername, int useportPort, string connectionId, SharePortCacheDto value = default!, CancellationToken cancellationToken = default!) => await _memoryCache.GetOrCreateAsync($"{useportUsername}_{useportPort}_{connectionId}",
            async ct => value,
            cancellationToken: cancellationToken);
}
