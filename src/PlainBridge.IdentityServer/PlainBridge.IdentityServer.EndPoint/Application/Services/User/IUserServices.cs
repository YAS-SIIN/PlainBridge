using PlainBridge.IdentityServer.EndPoint.Domain.Entities;
using PlainBridge.IdentityServer.EndPoint.DTOs;

namespace PlainBridge.IdentityServer.EndPoint.Application.Services.User;

public interface IUserServices
{
    Task<ApplicationUser> ChangePasswordAsync(ChangeUserPasswordRequestDto model);
    Task<ApplicationUser> CreateAsync(UserRequestDto model);
    Task LoginUserForTestAsync(UserLoginDto model);
    Task<ApplicationUser> UpdateAsync(UserRequestDto model);
}