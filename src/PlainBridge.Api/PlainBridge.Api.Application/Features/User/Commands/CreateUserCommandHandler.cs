

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.ExternalServices.Identity;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.Features.User.Commands;

public class CreateUserCommandHandler(ILogger<CreateUserCommandHandler> _logger, MainDbContext _dbContext, IIdentityService _identityService) : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user: {UserName}, {Email}", request.UserName, request.Email);
        var existedUser = await _dbContext.Users.AnyAsync(x => x.UserName.UserNameValue == request.UserName || x.Email.Equals(request.Email), cancellationToken);
        if (existedUser)
        {
            _logger.LogWarning("User already exists with username: {UserName} or email: {Email}", request.UserName, request.Email);
            throw new DuplicatedException($"{request.UserName} or {request.Email}");
        }

        UserRequest userDto = new()
        {
            UserId = request.ExternalId,
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password, 
            RePassword = request.RePassword, 
            PhoneNumber = request.PhoneNumber,
            Name = request.Name,
            Family = request.Family
        };

        var userCreationResult = await _identityService.CreateUserAsync(userDto, cancellationToken);

        if (userCreationResult is null || userCreationResult.ResultCode != ResultCodeEnum.Success || userCreationResult.Data is null)
        {
            _logger.LogError("User creation on identity server failed for username: {Username}", request.UserName);
            throw new ApplicationException("User creation on identity server failed");
        }

        var newUser = Domain.UserAggregate.User.Create(userCreationResult!.Data!, request.UserName, request.Email, request.PhoneNumber, request.Name, request.Family, request.ExternalId);

        var createdUser = await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        newUser = createdUser.Entity;
        _logger.LogInformation("User created successfully: {Username}", request.UserName);
        return newUser.AppId.ViewId;
    }
}
