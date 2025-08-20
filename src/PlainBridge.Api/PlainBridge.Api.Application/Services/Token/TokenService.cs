
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using PlainBridge.SharedApplication.Extensions;

namespace PlainBridge.Api.Application.Services.Token;

public class TokenService(HybridCache _hybridCache) : ITokenService
{
     
    public JwtSecurityToken ParseToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);

        return jwtSecurityToken;
    }

    public async Task<string> GenerateToken(string sub, string name, string family)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, $"{name} {family}"),
                    new Claim(ClaimTypes.GivenName, name),
                    new Claim(ClaimTypes.Surname, family),
                    new Claim("user_id", sub)
            }),
            Expires = DateTime.UtcNow.AddDays(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII
                .GetBytes("576537ae-b3da-4393-9233-68190b1646e3")), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

    public async Task<string?> SetGetTokenPSubAsync(string tokenp, string? value = default, CancellationToken cancellationToken = default!) => await _hybridCache.GetOrCreateAsync($"tokenpsub:{tokenp}", async ct => value, cancellationToken: cancellationToken);


    public async Task<string?> SetGetSubTokenAsync(string sub, string? value = default, CancellationToken cancellationToken = default!) => await
        _hybridCache.GetOrCreateAsync($"subtoken:{sub}", async ct => value, cancellationToken: cancellationToken);


    public async Task<string?> SetGetSubTokenPAsync(string sub, string value = default!, CancellationToken cancellationToken = default!) => await _hybridCache.GetOrCreateAsync($"subtokenp:{sub}", async ct => value, cancellationToken: cancellationToken);


    public async Task<string?> SetGetTokenPTokenAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default!) => await
        _hybridCache.GetOrCreateAsync($"tokenptoken:{tokenp}", async ct => value, cancellationToken: cancellationToken);
      
    public async Task<string?> SetGetSubIdTokenAsync(string sub, string value = default!, CancellationToken cancellationToken = default!)
    {
        string valueRes = default!;
        try
        {
            return await
          _hybridCache.GetOrCreateAsync($"subidtoken:{sub}", async ct => value, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            var aa = ex;
        }
        return valueRes;
    }
     

    public async Task<string?> SetGetTokenPRefreshTokenAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default!) => await _hybridCache.GetOrCreateAsync($"tokenprefreshtoken:{tokenp}", async ct => value, cancellationToken: cancellationToken);

      


}
