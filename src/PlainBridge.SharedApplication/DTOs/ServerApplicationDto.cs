

using PlainBridge.SharedApplication.Enums;

using System.ComponentModel.DataAnnotations;

namespace PlainBridge.SharedApplication.DTOs;

public record ServerApplicationDto : BaseDto<long>
{ 
    public Guid AppId { get; set; }
    public Guid? ServerApplicationViewId { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int InternalPort { get; set; }

    public ServerApplicationTypeEnum ServerApplicationType { get; set; }


}
