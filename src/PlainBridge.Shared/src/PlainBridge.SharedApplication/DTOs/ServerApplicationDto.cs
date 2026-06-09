

using PlainBridge.SharedApplication.Enums;

using System.ComponentModel.DataAnnotations;

namespace PlainBridge.SharedApplication.DTOs;

public record ServerApplicationDto : BaseDto<long>
{
    [Display(Name = "Application Id")]
    public Guid AppId { get; set; }

    [Display(Name = "Application app Id")]
    public string? ServerApplicationAppId { get; set; }

    [Display(Name = "User Id")]
    [Required]
    public long UserId { get; set; }
     
    [Display(Name = "User name")]
    public string? UserName { get; set; } = string.Empty;
     
    [Display(Name = "Application name")]
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Application internal port")]
    [Required]
    public int InternalPort { get; set; }

    public ServerApplicationTypeEnum ServerApplicationType { get; set; }
}
