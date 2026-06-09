using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Commands;

public class UpdateHostApplicationCommand : BaseCommand<long>, IRequest<Guid>
{
    [Display(Name = "Application Id")]
    public Guid AppId { get; set; }

    [Display(Name = "Name")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public required string Name { get; set; }
      
    [Display(Name = "Domain")]
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public required string Domain { get; set; }

    [Display(Name = "Internal URL")]
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public required string InternalUrl { get; set; }
     
    [Display(Name = "User Id")]
    [Required]
    public long UserId { get; set; }

}
