

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
 
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.Identity;
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

    public async Task<IList<UserDto>> GetAllAsync(CancellationToken cancellationToken)
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

    public async Task<Guid> CreateLocallyAsync(UserDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user: {Username}, {Email}", user.Username, user.Email);
        var existedUser = await _dbContext.Users.AnyAsync(x => x.Username == user.Username || x.Email == user.Email, cancellationToken);
        if (existedUser)
        {
            _logger.LogWarning("User already exists with username: {Username} or email: {Email}", user.Username, user.Email);
            throw new DuplicatedException($"{user.Username} or {user.Email}");
        }
        var newUser =  Domain.Entities.User.Create(user.ExternalId, user.Username, user.Email, user.PhoneNumber, user.Name, user.Family, user.ExternalId);
        
        var createdUser = await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        newUser = createdUser.Entity;
        _logger.LogInformation("User created successfully: {Username}", user.Username);
        return newUser.AppId.ViewId;
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

        if (userCreationResult is null || userCreationResult.ResultCode != ResultCodeEnum.Success || userCreationResult.Data is null)
        {
            _logger.LogError("User creation on identity server failed for username: {Username}", user.Username);
            throw new ApplicationException("User creation on identity server failed");
        }
        
        var newUser = Domain.Entities.User.Create(userCreationResult!.Data!, user.Username, user.Email, user.PhoneNumber, user.Name, user.Family, user.ExternalId);
 
        var createdUser = await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        newUser = createdUser.Entity;
        _logger.LogInformation("User created successfully: {Username}", user.Username);
        return newUser.AppId.ViewId;
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

    public async Task UpdateAsync(UserDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating for user id: {UserId}", user.Id);
        var existedUser = await _dbContext.Users.FindAsync(user.Id, cancellationToken);
        if (existedUser == null)
        {
            _logger.LogWarning("User not found for update. Id: {UserId}", user.Id);
            throw new NotFoundException(nameof(UserDto), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserDto.Id), user.Id) });
        }

        var userUpdatingResult = await _identityService.UpdateUserAsync(user, cancellationToken);

        if (userUpdatingResult.ResultCode != ResultCodeEnum.Success)
        {
            _logger.LogError("User update failed on identity server for user id: {UserId}", user.Id);
            throw new ApplicationException("Updating user on identity server failed");
        }

        existedUser.ExternalId = userUpdatingResult.Data;
        existedUser.Name = user.Name;
        existedUser.Family = user.Family;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Profile updated successfully for user id: {UserId}", user.Id);
    }
}
