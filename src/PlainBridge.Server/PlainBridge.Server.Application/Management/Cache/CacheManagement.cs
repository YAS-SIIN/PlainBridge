

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using PlainBridge.Server.Application.Management.WebSocketManagement;
using PlainBridge.SharedApplication.DTOs;

using System.Net.WebSockets;

namespace PlainBridge.Server.Application.Management.Cache;

public class CacheManagement : ICacheManagement
{
    private readonly ILogger<CacheManagement> _logger;
    private readonly IMemoryCache _memoryCache;

    public CacheManagement(ILogger<CacheManagement> logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public HostApplicationDto SetHostApplication(string host, HostApplicationDto value) => _memoryCache.Set($"hostApplication:{host}", value);
    public bool TryGetHostApplication(string host, out HostApplicationDto value) => _memoryCache.TryGetValue($"hostApplication:{host}", out value);

    public ServerApplicationDto SetServerApplication(Guid viewId, ServerApplicationDto value) => _memoryCache.Set($"serverApplication:viewId:{viewId}", value);

    public ServerApplicationDto SetServerApplication(int port, ServerApplicationDto value) => _memoryCache.Set($"serverApplication:port:{port}", value);
    public bool TryGetServerApplication(int port, out ServerApplicationDto value) => _memoryCache.TryGetValue($"serverApplication:port:{port}", out value);
    public bool TryGetServerApplication(Guid viewId, out ServerApplicationDto value) => _memoryCache.TryGetValue($"serverApplication:viewId:{viewId}", out value);

    public IWebSocketManagement SetWebSocket(string host, IWebSocketManagement value) => _memoryCache.Set($"webSocket:{host}", value);
    public bool TryGetWebSocket(string host, out IWebSocketManagement value) => _memoryCache.TryGetValue($"webSocket:{host}", out value);
    public void RemoveWebSocket(string host) => _memoryCache.Remove($"webSocket:{host}");

}
