using PlainBridge.Shared.Application.DTOs;

using System.ComponentModel.DataAnnotations;

namespace PlainBridge.Api.Infrastructure.DTOs;
public record ChangeUserPasswordRequest
{
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
