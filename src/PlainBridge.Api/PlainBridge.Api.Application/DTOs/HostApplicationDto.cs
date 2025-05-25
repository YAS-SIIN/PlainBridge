
using System.ComponentModel.DataAnnotations;

namespace PlainBridge.Api.Application.DTOs;

public record HostApplicationDto : BaseDto<long>
{
    public Guid AppId { get; set; }
    
    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    [Required]
    [StringLength(200)]
    public string Domain { get; set; }

    [Required]
    [StringLength(200)]
    public string InternalUrl { get; set; }

}
