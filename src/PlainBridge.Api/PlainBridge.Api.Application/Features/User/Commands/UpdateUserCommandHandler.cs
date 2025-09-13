

using Microsoft.Extensions.Logging; 
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.ExternalServices.Identity;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.Features.User.Commands;

public class UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> _logger, MainDbContext _dbContext, IIdentityService _identityService) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating for user id: {UserId}", request.Id);
        var existedUser = await _dbContext.Users.FindAsync(request.Id, cancellationToken);
        if (existedUser == null)
        {
            _logger.LogWarning("User not found for update. Id: {UserId}", request.Id);
            throw new NotFoundException(nameof(UserRequest), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserRequest.UserId), request.Id) });
        }

        UserRequest userDto = new()
        {
            UserId = existedUser.ExternalId,
            UserName = existedUser.UserName.UserNameValue,
            Email = existedUser.Email,
            //State = request.State,
            Name = request.Name,
            Family = request.Family,
        };
        var userUpdatingResult = await _identityService.UpdateUserAsync(userDto, cancellationToken);

        if (userUpdatingResult is null || userUpdatingResult.ResultCode != ResultCodeEnum.Success || userUpdatingResult.Data is null)
        {
            _logger.LogError("User creation on identity server failed for username: {Username}", existedUser.UserName);
            throw new ApplicationException("User creation on identity server failed");
        }

        existedUser.ExternalId = userUpdatingResult!.Data;
        existedUser.Name = request.Name;
        existedUser.Family = request.Family;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Profile updated successfully for user id: {UserId}", request.Id);
    }
}
