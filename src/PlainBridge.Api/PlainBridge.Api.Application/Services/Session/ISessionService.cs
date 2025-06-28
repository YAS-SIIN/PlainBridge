using PlainBridge.Api.Application.DTOs;

namespace PlainBridge.Api.Application.Services.Session
{
    public interface ISessionService
    {
        Task<UserProfileViewDto?> GetCurrentCustomerProfileAsync(CancellationToken cancellationToken);
        Task<UserDto> GetCurrentUsereAsync(CancellationToken cancellationToken);
    }
}