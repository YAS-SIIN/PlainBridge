
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
        _logger.LogInformation("Getting token");
        var client = _httpClientFactory.CreateClient("Api");

        // discover endpoints from metadata
        var uri = new Uri(_applicationSettings.Value.PlainBridgeIdsUrl!);
        var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = uri.ToString(),
            Policy =
                {
                    RequireHttps = _applicationSettings.Value.PlainBridgeUseHttp == false
                }
        }, cancellationToken);
        if (disco.IsError)
            throw new ApplicationException("Discovery not found");

        // request token
        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,

            ClientId = _applicationSettings.Value.PlainBridgeIdsClientId,
            ClientSecret = _applicationSettings.Value.PlainBridgeIdsClientSecret,
            Scope = _applicationSettings.Value.PlainBridgeIdsScope
        });

        return tokenResponse;
    }
}
