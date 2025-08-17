using System.IdentityModel.Tokens.Jwt;

namespace PlainBridge.Api.Application.Services.Token;

public interface ITokenService
{
    Task<string> GenerateToken(string sub, string name, string family);
    JwtSecurityToken ParseToken(string token);
    Task<string?> SetGetSubIdTokenAsync(string sub, string value = default!, CancellationToken cancellationToken = default);
    Task<string?> SetGetSubTokenAsync(string sub, string value = default!, CancellationToken cancellationToken = default);
    Task SetGetSubTokenPAsync(string sub, string value = default!, CancellationToken cancellationToken = default);
    Task<string?> SetGetTokenPRefreshTokenAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default);
    Task<string?> SetGetTokenPSubAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default);
    Task<string?> SetGetTokenPTokenAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default);
}