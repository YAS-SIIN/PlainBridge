 
using System.ComponentModel.DataAnnotations; 
using PlainBridge.Api.Infrastructure.DTOs; 
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.Features.User.Queries;

public class GetUserByExternalIdQuery : IRequest<UserDto>
{

    [Required]
    public string ExternalId { get; set; }

}
