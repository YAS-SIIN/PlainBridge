
using System.Runtime.Serialization;

namespace PlainBridge.Api.Application.DTOs;

public class ApplicationSettings
{
    public required string PlainBridgeIdsClientId { get; set; }
    public required string PlainBridgeIdsClientSecret { get; set; }
    public required string PlainBridgeIdsScope { get; set; }
    public required string PlainBridgeIdsUrl { get; set; }
    public required bool PlainBridgeUseHttp { get; set; }
    public required string PlainBridgeWebUrl { get; set; } 
    public required string PlainBridgeWebRedirectPage { get; set; }

    public required string HybridDistributedCacheExpirationTime { get; set; } = "24:00:00"; // Default to 24 hours
    public required string HybridMemoryCacheExpirationTime { get; set; } = "00:30:00"; // Default to 30 minutes
    public required int HybridCacheMaximumPayloadBytes { get; set; } = 10485760; //1024 * 1024 * 10; 10MB
    public required int HybridCacheMaximumKeyLength { get; set; } = 512;
}
