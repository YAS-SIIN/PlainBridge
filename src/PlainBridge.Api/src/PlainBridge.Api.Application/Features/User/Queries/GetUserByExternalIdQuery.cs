
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Infrastructure.DTOs; 
using PlainBridge.Shared.Application.Mediator;
using System.ComponentModel.DataAnnotations; 

namespace PlainBridge.Api.Application.Features.User.Queries;

public class GetUserByExternalIdQuery : IRequest<UserDto>
{

    [Required]
    public string ExternalId { get; set; }

}
