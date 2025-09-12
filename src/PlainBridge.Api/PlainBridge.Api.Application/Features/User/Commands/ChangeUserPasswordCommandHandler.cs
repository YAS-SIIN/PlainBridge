using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.ExternalServices.Identity;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainBridge.Api.Application.Features.User.Commands;

public class ChangeUserPasswordCommandHandler(ILogger<ChangeUserPasswordCommandHandler> _logger, MainDbContext _dbContext, IIdentityService _identityService) : IRequestHandler<ChangeUserPasswordCommand>
{
    public async Task Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing password for user id: {UserId}", request.Id);
        var user = await _dbContext.Users.FindAsync(request.Id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found for password change. Id: {UserId}", request.Id);
            throw new NotFoundException(nameof(UserRequest), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserRequest.Id), request.Id) });
        }
        ChangeUserPasswordRequest changeUserPasswordDto = new()
        {
            Id = request.Id,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };
        var userChangePasswordResult = await _identityService.ChangePasswordAsync(changeUserPasswordDto, cancellationToken);

        if (userChangePasswordResult.ResultCode != ResultCodeEnum.Success)
        {
            _logger.LogError("Password change failed on identity server for user id: {UserId}", request.Id);
            throw new ApplicationException("User creation on identity server failed");
        }

        _logger.LogInformation("Password changed successfully for user id: {UserId}", request.Id);
    }
}
