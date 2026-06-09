using IdentityModel.Client;

namespace PlainBridge.Server.Application.Services.Identity;

public interface IIdentityService
{
    Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken);
}