using System.ComponentModel.DataAnnotations;

namespace PlainBridge.IdentityServer.EndPoint.DTOs;

public record UserLoginDto
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}
