
using System.ComponentModel.DataAnnotations;

namespace PlainBridge.SharedApplication.DTOs;

public record HostApplicationDto : BaseDto<long>
{
    [Display(Name = "Application Id")]
    public Guid AppId { get; set; }

    [Display(Name = "Name")]
    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    [Display(Name = "Domain")]
    [Required]
    [StringLength(200)]
    public string Domain { get; set; }

    [Display(Name = "Internal URL")]
    [Required]
    [StringLength(200)]
    public string InternalUrl { get; set; }

    public string GetProjectHost(string defaultDomain) => $"{this.Domain}{defaultDomain}";
}
