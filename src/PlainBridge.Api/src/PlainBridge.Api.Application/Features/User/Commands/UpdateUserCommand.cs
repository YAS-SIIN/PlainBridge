
using PlainBridge.Api.Application.UseCases;
using PlainBridge.SharedApplication.Mediator;
using System.ComponentModel.DataAnnotations;

namespace PlainBridge.Api.Application.Features.User.Commands;

public class UpdateUserCommand : BaseCommand<long>, IRequest
{
    [Display(Name = "Application Id")]
    public Guid AppId { get; set; }
       
    [Display(Name = "Name")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; }

    [Display(Name = "Family name")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Family { get; set; }
}
