using System.IdentityModel.Tokens.Jwt;

namespace PlainBridge.Api.Application.Services.Token;

public interface ITokenService
{
    JwtSecurityToken ParseToken(string token);
    Task<string> GenerateToken(string sub, string name, string family);

    Task SetTokenPSubAsync(string tokenp, string? value = null, CancellationToken cancellationToken = default);
    Task<string?> GetTokenPSubAsync(string tokenp, CancellationToken cancellationToken = default);

    Task SetSubTokenAsync(string sub, string? value = null, CancellationToken cancellationToken = default);
    Task<string?> GetSubTokenAsync(string sub, CancellationToken cancellationToken = default);

    Task SetSubTokenPAsync(string sub, string value = null, CancellationToken cancellationToken = default);
    Task<string?> GetSubTokenPAsync(string sub, CancellationToken cancellationToken = default);

    Task SetTokenPTokenAsync(string tokenp, string value = null, CancellationToken cancellationToken = default);
    Task<string?> GetTokenPTokenAsync(string tokenp, CancellationToken cancellationToken = default);

    Task SetSubIdTokenAsync(string sub, string value = null, CancellationToken cancellationToken = default);
    Task<string?> GetSubIdTokenAsync(string sub, CancellationToken cancellationToken = default);

    Task SetTokenPRefreshTokenAsync(string tokenp, string value = null, CancellationToken cancellationToken = default);
    Task<string?> GetTokenPRefreshTokenAsync(string tokenp, CancellationToken cancellationToken = default);
}