

using System.ComponentModel.DataAnnotations;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class CreateServerApplicationCommand : BaseCommand<long>, IRequest<Guid>
{
    [Display(Name = "Application Id")]
    public Guid AppId { get; set; }

    [Display(Name = "Name")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public required string Name { get; set; }

    [Display(Name = "User Id")]
    [Required]
    public long UserId { get; set; }
     
    [Display(Name = "Domain")]
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public required string Domain { get; set; }

    [Display(Name = "Internal URL")]
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public required string InternalUrl { get; set; }

}
