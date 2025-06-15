

using PlainBridge.Api.Domain.Enums;

using System.ComponentModel.DataAnnotations;

namespace PlainBridge.SharedApplication.DTOs;

public record ServerApplicationDto : BaseDto<long>
{ 
    public Guid? AppId { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int InternalPort { get; set; }


}
