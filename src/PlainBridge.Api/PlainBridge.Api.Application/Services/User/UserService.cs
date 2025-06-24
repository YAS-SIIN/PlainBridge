

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.Identity;
using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.Application.Services.User;

public class UserService(
    ILogger<UserService> _logger,
    MainDbContext _dbContext,
    IIdentityService _identityService) : IUserService
{

    public async Task<UserDto> GetUserByExternalIdAsync(string externalId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by external id: {ExternalId}", externalId);
        var user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found with external id: {ExternalId}", externalId);
            throw new NotFoundException(nameof(UserDto), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserDto.ExternalId), externalId) });
        }

        _logger.LogInformation("User found with external id: {ExternalId}", externalId);
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

    public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken)
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

    public async Task<Guid> CreateAsync(UserDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user: {Username}, {Email}", user.Username, user.Email);
        var existedUser = await _dbContext.Users.AnyAsync(x => x.Username == user.Username || x.Email == user.Email, cancellationToken);
        if (existedUser)
        {
            _logger.LogWarning("User already exists with username: {Username} or email: {Email}", user.Username, user.Email);
            throw new DuplicatedException($"{user.Username} or {user.Email}");
        }

        var userCreationResult = await _identityService.CreateUserAsync(user, cancellationToken);

        if (userCreationResult.ResultCode != ResultCodeEnum.Success)
        {
            _logger.LogError("User creation on identity server failed for username: {Username}", user.Username);
            throw new ApplicationException("User creation on identity server failed");
        }

        var newUser = new Domain.Entities.User
        {
            AppId = Guid.NewGuid(),
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Name = user.Name,
            Family = user.Family,
            ExternalId = userCreationResult!.Data!
        };
        var createdUser = await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        newUser = createdUser.Entity;
        _logger.LogInformation("User created successfully: {Username}", user.Username);
        return newUser.AppId;
    }

    public async Task ChangePasswordAsync(ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing password for user id: {UserId}", changeUserPassword.Id);
        var user = await _dbContext.Users.FindAsync(changeUserPassword.Id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found for password change. Id: {UserId}", changeUserPassword.Id);
            throw new NotFoundException(nameof(UserDto), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserDto.Id), changeUserPassword.Id) });
        }

        var userChangePasswordResult = await _identityService.ChangePasswordAsync(changeUserPassword, cancellationToken);

        if (userChangePasswordResult.ResultCode != ResultCodeEnum.Success)
        {
            _logger.LogError("Password change failed on identity server for user id: {UserId}", changeUserPassword.Id);
            throw new ApplicationException("User creation on identity server failed");
        }

        _logger.LogInformation("Password changed successfully for user id: {UserId}", changeUserPassword.Id);
    }

    public async Task UpdateProfileAsync(UserDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating profile for user id: {UserId}", user.Id);
        var existedUser = await _dbContext.Users.FindAsync(user.Id, cancellationToken);
        if (existedUser == null)
        {
            _logger.LogWarning("User not found for profile update. Id: {UserId}", user.Id);
            throw new NotFoundException(nameof(UserDto), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserDto.Id), user.Id) });
        }

        var userUpdatingResult = await _identityService.UpdateUserAsync(user, cancellationToken);

        if (userUpdatingResult.ResultCode != ResultCodeEnum.Success)
        {
            _logger.LogError("Profile update failed on identity server for user id: {UserId}", user.Id);
            throw new ApplicationException("Updating profile on identity server failed");
        }

        existedUser.Name = user.Name;
        existedUser.Family = user.Family;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Profile updated successfully for user id: {UserId}", user.Id);
    }
}
