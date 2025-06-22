using System.ComponentModel.DataAnnotations;

namespace PlainBridge.IdentityServer.EndPoint.Dto;

public class ChangePasswordDto
{
    [Display(Name = "User Id")]
    [Required(ErrorMessage = "{0} is required.")]
    public string UserId { get; set; }

    [Display(Name = "Current password")]
    [Required(ErrorMessage = "{0} is required.")]
    public string CurrentPassword { get; set; }

    [Display(Name = "New password")]
    [Required(ErrorMessage = "{0} is required.")]
    public string NewPassword { get; set; }
}
