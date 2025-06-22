

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
        var user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(UserDto), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserDto.ExternalId), externalId) });

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
        var res = await _dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
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
        var existedUser = await _dbContext.Users.AnyAsync(x => x.Username == user.Username || x.Email == user.Email, cancellationToken);
        if (existedUser) throw new DuplicatedException($"{user.Username} or {user.Email}");



        var userCreationResult = await _identityService.CreateUserAsync(user, cancellationToken);

        if (userCreationResult.ResultCode != ResultCodeEnum.Success)
            throw new ApplicationException("User creation on identity server failed");


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
        await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user.AppId;
    }

    public async Task ChangePasswordAsync(ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(changeUserPassword.Id, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(UserDto), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserDto.Id), changeUserPassword.Id) });

        var userChangePasswordResult = await _identityService.ChangePasswordAsync(changeUserPassword, cancellationToken);

        if (userChangePasswordResult.ResultCode != ResultCodeEnum.Success)
            throw new ApplicationException("User creation on identity server failed");

    }

    public async Task UpdateProfileAsync(UserDto user, CancellationToken cancellationToken)
    {
        var existedUser = await _dbContext.Users.FindAsync(user.Id, cancellationToken);
        if (existedUser == null)
            throw new NotFoundException(nameof(UserDto), new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(nameof(UserDto.Id), user.Id) });


        var userUpdatingResult = await _identityService.UpdateUserAsync(user, cancellationToken);

        if (userUpdatingResult.ResultCode != ResultCodeEnum.Success)
            throw new ApplicationException("Updating profile on identity server failed");

        existedUser.Name = user.Name;
        existedUser.Family = user.Family;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
