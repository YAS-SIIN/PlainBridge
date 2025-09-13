using System.ComponentModel.DataAnnotations;

namespace PlainBridge.IdentityServer.EndPoint.DTOs;

public record ChangeUserPasswordRequestDto
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

    [Display(Name = "Confirm password")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string RePassword { get; set; }

}
