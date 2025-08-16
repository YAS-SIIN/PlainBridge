

using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.WebSocketManagement;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Management.Cache;

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

    public async Task<ServerApplicationDto> SetGetServerApplicationAsync(Guid appId, ServerApplicationDto value = default!, CancellationToken cancellationToken = default! ) => await
        _memoryCache.GetOrCreateAsync($"serverApplication:appId:{appId}",
            async ct => value,
            options,
            cancellationToken: cancellationToken);

    public async Task<ServerApplicationDto> SetGetServerApplicationAsync(string username, int port, ServerApplicationDto value = default!, CancellationToken cancellationToken = default!) => await
        _memoryCache.GetOrCreateAsync($"serverApplication:username:{username}:port:{port}",
            async ct => value,
            options,
            cancellationToken: cancellationToken);

    public async Task<IWebSocketManagement> SetGetWebSocketAsync(string host, IWebSocketManagement value = default!, CancellationToken cancellationToken = default!) => await
        _memoryCache.GetOrCreateAsync($"webSocket:{host}",
            async ct => value,
            options,
            cancellationToken: cancellationToken);

    public async Task RemoveWebSocketAsync(string host, CancellationToken cancellationToken = default!) => await _memoryCache.RemoveAsync($"webSocket:{host}", cancellationToken);

}
