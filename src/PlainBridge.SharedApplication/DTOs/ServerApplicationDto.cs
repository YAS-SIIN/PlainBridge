

using PlainBridge.SharedApplication.Enums;

using System.ComponentModel.DataAnnotations;

namespace PlainBridge.SharedApplication.DTOs;

public record ServerApplicationDto : BaseDto<long>
{
    [Display(Name = "Application Id")]
    public Guid AppId { get; set; }

    [Display(Name = "Application app Id")]
    public Guid? ServerApplicationAppId { get; set; }

    [Display(Name = "Application name")]
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Application internal port")]
    [Required]
    public int InternalPort { get; set; }

    public ServerApplicationTypeEnum ServerApplicationType { get; set; }


}
