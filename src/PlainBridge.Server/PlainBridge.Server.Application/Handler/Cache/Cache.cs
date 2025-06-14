

using Microsoft.Extensions.Caching.Memory;

using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Handler.Cache;

public class Cache : ICache
{
    private readonly IMemoryCache _memoryCache;

    public Cache(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public HostApplicationDto SetHostApplication(string host, HostApplicationDto value) => _memoryCache.Set($"hostApplication:{host}", value);
    public bool TryGetHostApplication(string host, out HostApplicationDto value) => _memoryCache.TryGetValue($"hostApplication:{host}", out value);

    public ServerApplicationDto SetServerApplication(string username, int port, ServerApplicationDto value) => _memoryCache.Set($"serverApplication:username:{username}:port:{port}", value);
    public bool TryGetServerApplication(string username, int port, out ServerApplicationDto value) => _memoryCache.TryGetValue($"serverApplication:username:{username}:port:{port}", out value);
    public ServerApplicationDto SetServerApplication(Guid viewId, ServerApplicationDto value) => _memoryCache.Set($"serverApplication:viewId:{viewId}", value);
    public bool TryGetServerApplication(Guid viewId, out ServerApplicationDto value) => _memoryCache.TryGetValue($"serverApplication:viewId:{viewId}", out value);

}
