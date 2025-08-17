using System.IdentityModel.Tokens.Jwt;

namespace PlainBridge.Api.Application.Services.Token;

public interface ITokenService
{
    JwtSecurityToken ParseToken(string token);
    Task<string> GenerateToken(string sub, string name, string family);

    Task<string?> SetGetTokenPSubAsync(string tokenp, string? value = null, CancellationToken cancellationToken = default);
 
    Task<string?> SetGetSubTokenAsync(string sub, string? value = null, CancellationToken cancellationToken = default);
    
    Task<string?> SetGetSubTokenPAsync(string sub, string value = null, CancellationToken cancellationToken = default);
   
    Task<string?> SetGetTokenPTokenAsync(string tokenp, string value = null, CancellationToken cancellationToken = default);
  
    Task<string?> SetGetSubIdTokenAsync(string sub, string value = null, CancellationToken cancellationToken = default);
  
    Task<string?> SetGetTokenPRefreshTokenAsync(string tokenp, string value = null, CancellationToken cancellationToken = default); 
}