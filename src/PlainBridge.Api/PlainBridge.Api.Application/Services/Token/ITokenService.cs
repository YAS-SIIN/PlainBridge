using System.IdentityModel.Tokens.Jwt;

namespace PlainBridge.Api.Application.Services.Token;

public interface ITokenService
{
    Task<string> GenerateToken(string sub, string name, string family);
    Task<string?> GetSubIdTokenAsync(string sub);
    Task<string?> GetSubTokenPAsync(string sub);
    Task<string?> GetSunTokenAsync(string sub);
    Task<string?> GetTokenPRefreshTokenAsync(string tokenp);
    Task<string?> GetTokenPSubAsync(string tokenp);
    Task<string?> GetTokenPTokenAsync(string tokenp);
    JwtSecurityToken ParseToken(string token);
    Task SetSubIdTokenAsync(string sub, string idToken);
    Task SetSubTokenAsync(string sub, string token);
    Task SetSubTokenPAsync(string sub, string tokenp);
    Task SetTokenPRefreshTokenAsync(string tokenp, string refreshToken);
    Task SetTokenPSubAsync(string tokenp, string sub);
    Task SetTokenPTokenAsync(string tokenp, string token);
}