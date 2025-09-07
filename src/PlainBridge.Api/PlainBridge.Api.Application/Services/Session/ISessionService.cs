
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Application.Services.Session
{
    public interface ISessionService
    {
        Task<UserProfileViewDto?> GetCurrentUserProfileAsync(CancellationToken cancellationToken);
        Task<UserDto> GetCurrentUserAsync(CancellationToken cancellationToken);
        Task RefreshTokenAsync(CancellationToken cancellationToken);
    }
}