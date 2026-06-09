using System.IdentityModel.Tokens.Jwt;

namespace PlainBridge.Api.Application.Services.Token;

public interface ITokenService
{
    JwtSecurityToken ParseToken(string token);
    Task<string> GenerateToken(string sub, string name, string family);

}