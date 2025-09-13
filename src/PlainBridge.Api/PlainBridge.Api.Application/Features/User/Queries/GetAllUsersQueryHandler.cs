

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.Features.User.Queries;

public class GetAllUsersQueryHandler(ILogger<GetAllUsersQueryHandler> _logger, MainDbContext _dbContext) : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users.");
        var res = await _dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
        _logger.LogInformation("Found {UserCount} users.", res.Count);
        return res.Select(x => new UserDto(x.AppId.ViewId, x.ExternalId, x.UserName.UserNameValue, x.Email, x.PhoneNumber, x.Name, x.Family)).ToList();
    }

}
