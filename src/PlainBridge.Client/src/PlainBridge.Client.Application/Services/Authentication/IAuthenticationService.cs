
using PlainBridge.Shared.Application.DTOs;

namespace PlainBridge.Client.Application.Services.Authentication;

public interface IAuthenticationService
{
    Task<UserProfileViewDto> LoginAsync(CancellationToken cancellationToken);

    Task LogoutAsync(CancellationToken cancellationToken);
}