 
using Microsoft.Extensions.Caching.Hybrid; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.WebSocketManagement; 
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Management.Cache;

public class CacheManagement(ILogger<CacheManagement> _logger, HybridCache _hybridCache, IOptions<ApplicationSettings> _appSettings) : ICacheManagement
{ 
    public async Task<HostApplicationDto?> SetGetHostApplicationAsync(string host, HostApplicationDto value = default!, CancellationToken cancellationToken = default!) => await _hybridCache.GetOrCreateAsync(
            $"hostApplication:{host}",
            async ct => value, 
            cancellationToken: cancellationToken);

    public async Task<ServerApplicationDto?> SetGetServerApplicationAsync(string appId, ServerApplicationDto value = default!, CancellationToken cancellationToken = default! ) => await
        _hybridCache.GetOrCreateAsync($"serverApplication:appId:{appId}",
            async ct => value, 
            cancellationToken: cancellationToken);

    public async Task<ServerApplicationDto?> SetGetServerApplicationAsync(string username, int port, ServerApplicationDto value = default!, CancellationToken cancellationToken = default!) => await
        _hybridCache.GetOrCreateAsync($"serverApplication:username:{username}:port:{port}",
            async ct => value, 
            cancellationToken: cancellationToken);

    public async Task<IWebSocketManagement?> SetGetWebSocketAsync(string host, IWebSocketManagement value = default!, CancellationToken cancellationToken = default!) => await
        _hybridCache.GetOrCreateAsync($"webSocket:{host}",
            async ct => value,
            cancellationToken: cancellationToken);

    public async Task RemoveWebSocketAsync(string host, CancellationToken cancellationToken = default!) => await _hybridCache.RemoveAsync($"webSocket:{host}", cancellationToken);

}
