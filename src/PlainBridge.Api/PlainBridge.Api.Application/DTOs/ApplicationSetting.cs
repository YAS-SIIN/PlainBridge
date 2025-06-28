
using System.Runtime.Serialization;

namespace PlainBridge.Api.Application.DTOs;

public class ApplicationSetting
{
    public string IdsUrl { get; set; }
    public string IdsClientId { get; set; }
    public string IdsClientSecret { get; set; }
    public string IdsScope { get; set; }
    public string? ZIRALINK_USE_HTTP { get; set; }
    public string ZIRALINK_URL_IDS { get; set; }
}
