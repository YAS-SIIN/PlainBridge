using System.ComponentModel.DataAnnotations;

namespace PlainBridge.IdentityServerNew.EndPoint.DTOs;

public record UserLoginDto
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}
