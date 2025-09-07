using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class UpdateServerApplicationCommand : BaseCommand<long>, IRequest<Guid>
{
    [Display(Name = "Application Id")]
    public Guid AppId { get; set; }

    [Display(Name = "Application app Id")]
    public string? ServerApplicationAppId { get; set; }

    [Display(Name = "User Id")]
    [Required]
    public long UserId { get; set; }

    [Display(Name = "Application name")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Application internal port")]
    [Required]
    public int InternalPort { get; set; }

    public ServerApplicationTypeEnum ServerApplicationType { get; set; }

}
