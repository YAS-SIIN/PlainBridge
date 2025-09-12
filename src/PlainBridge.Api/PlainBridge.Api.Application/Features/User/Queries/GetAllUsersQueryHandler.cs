

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.Features.User.Queries;

public class GetAllUsersQueryHandler(ILogger<GetAllUsersQuery> _logger, MainDbContext _dbContext) : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users.");
        var res = await _dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
        _logger.LogInformation("Found {UserCount} users.", res.Count);
        return res.Select(x => new UserDto
        {
            Username = x.Username,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            Name = x.Name,
            Family = x.Family,
            ExternalId = x.ExternalId
        }).ToList();
    }

}
