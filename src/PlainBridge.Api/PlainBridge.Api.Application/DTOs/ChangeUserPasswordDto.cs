using PlainBridge.SharedApplication.DTOs;

using System.ComponentModel.DataAnnotations;

namespace PlainBridge.Api.Application.DTOs;

public record ChangeUserPasswordDto : BaseDto<long>
{ 
     
    [Display(Name = "Current password")]
    [Required]
    public string CurrentPassword { get; set; }

    [Display(Name = "New password")]
    [Required]
    public string NewPassword { get; set; }
}
