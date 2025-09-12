using PlainBridge.SharedApplication.DTOs;

using System.ComponentModel.DataAnnotations;

namespace PlainBridge.Api.Infrastructure.DTOs;
public record ChangeUserPasswordDto
{
    public long Id { get; set; }

    [Display(Name = "Current password")]
    [Required]
    public string CurrentPassword { get; set; }

    [Display(Name = "New password")]
    [Required]
    public string NewPassword { get; set; }
}
