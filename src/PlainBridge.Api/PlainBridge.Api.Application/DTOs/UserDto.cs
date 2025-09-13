using PlainBridge.SharedApplication.DTOs;
 
namespace PlainBridge.Api.Application.DTOs;

public record UserDto : BaseDto<long>
{ 
    public Guid AppId { get; set; }
    public string ExternalId { get; set; }
    public string UserName { get; set; } 
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }     
    public string Name { get; set; } 
    public string Family { get; set; }

    public UserDto(Guid appId, string externalId, string userName, string email, string? phoneNumber, string name, string family)
    {
        AppId = appId;
        ExternalId = externalId;
        UserName = userName;
        Email = email;
        PhoneNumber = phoneNumber;
        Name = name;
        Family = family;
    }
}
