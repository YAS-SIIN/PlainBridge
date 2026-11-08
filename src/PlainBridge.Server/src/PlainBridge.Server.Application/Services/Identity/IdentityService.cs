
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Server.Application.DTOs;

namespace PlainBridge.Server.Application.Services.Identity;

public class IdentityService(ILogger<IdentityService> _logger, IHttpClientFactory _httpClientFactory, IOptions<ApplicationSettings> _applicationSettings) : IIdentityService
{
    public async Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("IdentityServer");

        var tokenResponse = await client.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = "connect/token",
                ClientId = _applicationSettings.Value.PlainBridgeIdsClientId,
                ClientSecret = _applicationSettings.Value.PlainBridgeIdsClientSecret,
                Scope = _applicationSettings.Value.PlainBridgeIdsScope
            },
            cancellationToken);

        if (tokenResponse.IsError)
        {
            _logger.LogError("IdentityServer token request failed: {Error}", tokenResponse.Error);
            throw new ApplicationException($"Token request failed: {tokenResponse.Error}");
        }

        return tokenResponse;
    }
}
