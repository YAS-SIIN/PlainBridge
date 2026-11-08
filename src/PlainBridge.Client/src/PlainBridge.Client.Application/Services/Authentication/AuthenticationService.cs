 

namespace PlainBridge.Client.Application.Services.Authentication;

using Duende.IdentityModel.OidcClient;

using Microsoft.Extensions.Options;

using PlainBridge.Client.Application.DTOs;
using PlainBridge.Shared.Application.DTOs;

using System.Security.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly OidcClient _oidcClient;

    public AuthenticationService(IOptions<ApplicationSettings> _applicationSettings)
    {
        var options = new OidcClientOptions
        {
            Authority = _applicationSettings.Value.PlainBridgeIdsUrl,
            ClientId = "client",
            Scope = "openid profile email PlainBridge offline_access",
            RedirectUri = "https://api-endpoint-plainbridge.dev.localhost:7498/callback",
            Browser = new LocalBrowser(5005)
        };

        _oidcClient = new OidcClient(options);
    }

    public async Task<UserProfileViewDto> LoginAsync(CancellationToken cancellationToken)
    {
        var result = await _oidcClient.LoginAsync(new LoginRequest(), cancellationToken);

        if (result.IsError) throw new AuthenticationException(result.Error);

        // Token storage and user profile retrieval
        // will be implemented in the next step.

        return new UserProfileViewDto();
    }
     
    public Task LogoutAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}