
using System.Runtime.Serialization;

namespace PlainBridge.IdentityServer.EndPoint.DTOs;

public class ApplicationSettings
{
    public required string PlainBridgeApiUrl { get; set; }
    public required string PlainBridgeWebUrl { get; set; }
    public required string PlainBridgeIdsClientSecret { get; set; } 
}
