using PlainBridge.SharedApplication.DTOs;
 
namespace PlainBridge.Api.Application.DTOs;

public record UserDto : BaseDto<long>
{ 
    public Guid AppId { get; set; }
    public string ExternalId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }     
    public string Name { get; set; } 
    public string Family { get; set; }
}
