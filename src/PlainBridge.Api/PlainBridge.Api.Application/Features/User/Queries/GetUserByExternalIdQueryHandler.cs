

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.Features.User.Queries;

public class GetUserByExternalIdQueryHandler(ILogger<GetUserByExternalIdQueryHandler> _logger, MainDbContext _dbContext) : IRequestHandler<GetUserByExternalIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByExternalIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by external id: {ExternalId}", request.ExternalId);
        var user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.ExternalId == request.ExternalId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found with external id: {ExternalId}", request.ExternalId);
            throw new NotFoundException(nameof(UserDto), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserDto.ExternalId), request.ExternalId) });
        }

        _logger.LogInformation("User found with external id: {ExternalId}", request.ExternalId);
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Name = user.Name,
            Family = user.Family,
            ExternalId = user.ExternalId
        };
    }
}
