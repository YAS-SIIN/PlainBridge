using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Application.Identity.Customer;

public interface IIdentityService
{
    Task<ResultDto<string>> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
    Task<ResultDto<string>> CreateUserAsync(string username, string password, string email, string name, string family, CancellationToken cancellationToken);
    Task<ResultDto<string>> UpdateUserAsync(string userId, string name, string family, CancellationToken cancellationToken);
}