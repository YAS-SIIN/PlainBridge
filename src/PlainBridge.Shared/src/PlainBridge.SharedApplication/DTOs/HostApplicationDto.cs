
using System.ComponentModel.DataAnnotations;

namespace PlainBridge.SharedApplication.DTOs;

public record HostApplicationDto : BaseDto<long>
{
    public Guid AppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string InternalUrl { get; set; } = string.Empty;

    public string GetProjectHost(string? defaultDomain) => $"{Domain.ToLower()}{defaultDomain}";
}
