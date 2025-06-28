using PlainBridge.Api.Application.DTOs;

namespace PlainBridge.Api.Application.Services.Session
{
    public interface ISessionService
    {
        Task<UserProfileViewDto?> GetCurrentUserProfileAsync(CancellationToken cancellationToken);
        Task<UserDto> GetCurrentUserAsync(CancellationToken cancellationToken);
    }
}