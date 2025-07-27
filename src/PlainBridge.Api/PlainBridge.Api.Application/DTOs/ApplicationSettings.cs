
using System.Runtime.Serialization;

namespace PlainBridge.Api.Application.DTOs;

public class ApplicationSettings
{
    public required string PlainBridgeIdsClientId { get; set; }
    public required string PlainBridgeIdsClientSecret { get; set; }
    public required string PlainBridgeIdsScope { get; set; }
    public required bool PlainBridgeUseHttp { get; set; }
    public required string PlainBridgeIdsUrl { get; set; }
    public required string PlainBridgeWebUrl { get; set; } 
    public required string PlainBridgeWebRedirectPage { get; set; }
}
