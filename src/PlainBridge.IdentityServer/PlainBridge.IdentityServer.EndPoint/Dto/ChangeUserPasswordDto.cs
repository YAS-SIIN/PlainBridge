using System.ComponentModel.DataAnnotations;

namespace PlainBridge.IdentityServer.EndPoint.Dto;

public record ChangeUserPasswordDto
{
    [Display(Name = "User Id")]
    [Required]
    public string UserId { get; set; }

    [Display(Name = "Current password")]
    [Required]
    public string CurrentPassword { get; set; }

    [Display(Name = "New password")]
    [Required]
    public string NewPassword { get; set; }
}
