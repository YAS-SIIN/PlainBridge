

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.ExternalServices.Identity;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.Features.User.Commands;

public class CreateUserCommandHandler(ILogger<CreateUserCommand> _logger, MainDbContext _dbContext, IIdentityService _identityService) : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user: {Username}, {Email}", request.Username, request.Email);
        var existedUser = await _dbContext.Users.AnyAsync(x => x.Username == request.Username || x.Email == request.Email, cancellationToken);
        if (existedUser)
        {
            _logger.LogWarning("User already exists with username: {Username} or email: {Email}", request.Username, request.Email);
            throw new DuplicatedException($"{request.Username} or {request.Email}");
        }

        UserDto userDto = new()
        {
            Id = request.Id,
            AppId = request.AppId,
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            Description = request.Description,
            RePassword = request.RePassword,
            State = request.State,
            PhoneNumber = request.PhoneNumber,
            Name = request.Name,
            Family = request.Family,
            ExternalId = request.ExternalId
        };

        var userCreationResult = await _identityService.CreateUserAsync(userDto, cancellationToken);

        if (userCreationResult is null || userCreationResult.ResultCode != ResultCodeEnum.Success || userCreationResult.Data is null)
        {
            _logger.LogError("User creation on identity server failed for username: {Username}", request.Username);
            throw new ApplicationException("User creation on identity server failed");
        }

        var newUser = Domain.Entities.User.Create(userCreationResult!.Data!, request.Username, request.Email, request.PhoneNumber, request.Name, request.Family, request.ExternalId);

        var createdUser = await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        newUser = createdUser.Entity;
        _logger.LogInformation("User created successfully: {Username}", request.Username);
        return newUser.AppId.ViewId;
    }
}
