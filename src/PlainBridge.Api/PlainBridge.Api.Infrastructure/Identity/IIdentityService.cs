
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Infrastructure.Identity;

public interface IIdentityService
{
    Task<ResultDto<string>> ChangePasswordAsync(ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken);
    Task<ResultDto<string>> CreateUserAsync(UserDto user, CancellationToken cancellationToken);
    Task<ResultDto<string>> UpdateUserAsync(UserDto user, CancellationToken cancellationToken);
}