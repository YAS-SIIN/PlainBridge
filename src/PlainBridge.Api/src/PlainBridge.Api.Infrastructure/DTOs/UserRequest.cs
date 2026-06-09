using PlainBridge.SharedApplication.DTOs;

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace PlainBridge.Api.Infrastructure.DTOs;

public record UserRequest
{
    public string UserId { get; set; }
     
    [Display(Name = "Username")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string UserName { get; set; }

    [Display(Name = "Password")]
    [Required] 
    public string Password { get; set; }

    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string RePassword { get; set; }

    [Display(Name = "Email")]
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Email { get; set; }

    [Display(Name = "Phone number")]
    [StringLength(20)]
    [RegularExpression(@"^\+?[0-9\s\-()]+$", ErrorMessage = "Invalid phone number format.")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Name")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; }

    [Display(Name = "Family name")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Family { get; set; }
}
