
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Client.Application.DTOs;
using PlainBridge.Shared.Application.DTOs;
using System.Net.Http.Json;
using System.Security.Authentication;

namespace PlainBridge.Client.Application.Services.Authentication;

public class AuthenticationService(ILogger<AuthenticationService> _logger, IOptions<ApplicationSettings> _applicationSettings, IHttpClientFactory _httpClientFactory) : IAuthenticationService
{
    private OidcClient _oidcClient;

    public async Task<UserProfileViewDto> LoginAsync(CancellationToken cancellationToken)
    { 
        var httpClient = _httpClientFactory.CreateClient("IdentityServer");
        var response = await httpClient.GetAsync(".well-known/openid-configuration",cancellationToken);
        response.EnsureSuccessStatusCode();

        var discovery = await response.Content.ReadFromJsonAsync<DiscoveryResponse>(cancellationToken: cancellationToken);

        var browser = new LocalBrowser();

        var options = new OidcClientOptions
        {
            Authority = discovery.Issuer,
            ClientId = "client",
            Scope = "openid profile email PlainBridge offline_access",
            RedirectUri = browser.RedirectUri,
            Browser = browser
        };

        _oidcClient = new OidcClient(options);

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