using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Infrastructure.ExternalServices.Identity;

public interface IIdentityService
{
    Task<ResultDto<string>> ChangePasswordAsync(ChangeUserPasswordRequest changeUserPassword, CancellationToken cancellationToken);
    Task<ResultDto<string>> CreateUserAsync(UserRequest user, CancellationToken cancellationToken);
    Task<ResultDto<string>> UpdateUserAsync(UserRequest user, CancellationToken cancellationToken);
}