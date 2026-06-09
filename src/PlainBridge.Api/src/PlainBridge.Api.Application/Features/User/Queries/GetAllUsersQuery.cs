

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.Features.User.Queries;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{

}
