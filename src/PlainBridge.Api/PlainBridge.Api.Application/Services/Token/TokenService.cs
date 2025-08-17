
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

    public async Task SetTokenPSubAsync(string tokenp, string? value = default, CancellationToken cancellationToken = default!) => await _hybridCache.SetAsync($"tokenpsub:{tokenp}", value, cancellationToken: cancellationToken);


    public async Task<string?> GetTokenPSubAsync(string tokenp, CancellationToken cancellationToken = default!) => await
        _hybridCache.TryGetValueAsync<string>($"tokenpsub:{tokenp}", cancellationToken);



    public async Task SetSubTokenAsync(string sub, string? value = default, CancellationToken cancellationToken = default!) => await
        _hybridCache.SetAsync($"subtoken:{sub}", value, cancellationToken: cancellationToken);

    public async Task<string?> GetSubTokenAsync(string sub, CancellationToken cancellationToken = default!) => await 
        _hybridCache.TryGetValueAsync<string>($"subtoken:{sub}",cancellationToken);


    public async Task SetSubTokenPAsync(string sub, string value = default!, CancellationToken cancellationToken = default!) => await _hybridCache.SetAsync($"subtokenp:{sub}", value, cancellationToken: cancellationToken);

    public async Task<string?> GetSubTokenPAsync(string sub, CancellationToken cancellationToken = default!) => await _hybridCache.TryGetValueAsync<string>( $"subtokenp:{sub}", cancellationToken);


    public async Task SetTokenPTokenAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default!) => await
        _hybridCache.SetAsync($"tokenptoken:{tokenp}", value, cancellationToken: cancellationToken);

    public async Task<string?> GetTokenPTokenAsync(string tokenp, CancellationToken cancellationToken = default!) => await
        _hybridCache.TryGetValueAsync<string>($"tokenptoken:{tokenp}", cancellationToken);


    public async Task SetSubIdTokenAsync(string sub, string value = default!, CancellationToken cancellationToken = default!) => await
        _hybridCache.SetAsync($"subidtoken:{sub}", value, cancellationToken: cancellationToken);

    public async Task<string?> GetSubIdTokenAsync(string sub, CancellationToken cancellationToken = default!) => await _hybridCache.TryGetValueAsync<string>($"subidtoken:{sub}", cancellationToken);


    public async Task SetTokenPRefreshTokenAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default!) => await _hybridCache.SetAsync($"tokenprefreshtoken:{tokenp}", value, cancellationToken: cancellationToken);


    public async Task<string?> GetTokenPRefreshTokenAsync(string tokenp, CancellationToken cancellationToken = default!) => await _hybridCache.TryGetValueAsync<string>($"tokenprefreshtoken:{tokenp}", cancellationToken);



}
