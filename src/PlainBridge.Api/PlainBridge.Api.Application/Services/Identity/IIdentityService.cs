using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Application.Services.Identity;

public interface IIdentityService
{
    Task<ResultDto<string>> ChangePasswordAsync(ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken);
    Task<ResultDto<string>> CreateUserAsync(UserDto user, CancellationToken cancellationToken);
    Task<ResultDto<string>> UpdateUserAsync(UserDto user, CancellationToken cancellationToken);
}