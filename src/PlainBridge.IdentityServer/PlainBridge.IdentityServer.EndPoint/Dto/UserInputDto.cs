using System.ComponentModel.DataAnnotations;

namespace ZiraLink.IDS.Pages.Account.Register;

public class UserInputDto
{

    [Display(Name = "User Id")]
    public string UserId { get; set; }

    [Display(Name = "User name")]
    [Required(ErrorMessage = "{0} is required.")]
    [StringLength(100, ErrorMessage = "Username must be between 3 and 100 characters.", MinimumLength = 3)]
    public string Username { get; set; }

    [Display(Name = "Password")]
    [Required(ErrorMessage = "{0} is required.")]
    [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters.", MinimumLength = 6)]
    public string Password { get; set; }

    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string RePassword { get; set; }

    [Display(Name = "Email address")]
    [Required(ErrorMessage = "{0} is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Display(Name = "Name")]
    [Required(ErrorMessage = "{0} is required.")]
    [StringLength(100, ErrorMessage = "Name must be between 1 and 100 characters.", MinimumLength = 1)]
    public string Name { get; set; }

    [Display(Name = "Family name")]
    [Required(ErrorMessage = "{0} is required.")]
    [StringLength(100, ErrorMessage = "Family name must be between 1 and 100 characters.", MinimumLength = 1)]
    public string Family { get; set; }
}
