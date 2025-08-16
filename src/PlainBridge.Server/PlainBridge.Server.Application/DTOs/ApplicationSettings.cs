 

namespace PlainBridge.Server.Application.DTOs;

public class ApplicationSettings
{
    public required string DefaultDomain { get; set; }
    public required string PlainBridgeApiAddress { get; set; }
    public required string PlainBridgeIdsClientId { get; set; }
    public required string PlainBridgeIdsClientSecret { get; set; }
    public required string PlainBridgeIdsScope { get; set; }
    public required string PlainBridgeIdsUrl { get; set; }
    public required bool PlainBridgeUseHttp { get; set; }
    public required string DistributedCacheExpirationTime { get; set; } = "24:00:00"; // Default to 24 hours
    public required string MemoryCacheExpirationTime { get; set; } = "00:30:00"; // Default to 30 minutes


}
