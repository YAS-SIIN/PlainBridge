
using System.Runtime.Serialization;

namespace PlainBridge.Api.Application.DTOs;

public class ApplicationSetting
{
    public string IdsClientId { get; set; }
    public string IdsClientSecret { get; set; }
    public string IdsScope { get; set; }
    public string? PlainBridgeUseHttp { get; set; }
    public string PlainBridgeIdsUrl { get; set; }
}
