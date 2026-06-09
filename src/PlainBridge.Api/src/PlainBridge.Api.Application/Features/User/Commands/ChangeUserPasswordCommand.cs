

using PlainBridge.Api.Application.UseCases;
using PlainBridge.SharedApplication.Mediator;
using System.ComponentModel.DataAnnotations;

namespace PlainBridge.Api.Application.Features.User.Commands;

public class ChangeUserPasswordCommand : BaseCommand<long>, IRequest
{
    [Display(Name = "Current password")]
    [Required]
    public string CurrentPassword { get; set; }

    [Display(Name = "New password")]
    [Required]
    public string NewPassword { get; set; }
}
