
using Microsoft.IdentityModel.Tokens;

using StackExchange.Redis;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlainBridge.Api.Application.Services.Token;

public class TokenService : ITokenService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    private readonly IDatabase _db;

    public TokenService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _db = _connectionMultiplexer.GetDatabase(10);
    }

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

    public async Task SetTokenPSubAsync(string tokenp, string sub) => await _db.StringSetAsync($"tokenpsub:{tokenp}", sub);


    public async Task<string?> GetTokenPSubAsync(string tokenp) => await _db.StringGetAsync($"tokenpsub:{tokenp}");


    public async Task SetSubTokenAsync(string sub, string token) => await _db.StringSetAsync($"subtoken:{sub}", token);


    public async Task<string?> GetSunTokenAsync(string sub) => await _db.StringGetAsync($"subtoken:{sub}");


    public async Task SetSubTokenPAsync(string sub, string tokenp) => await _db.StringSetAsync($"subtokenp:{sub}", tokenp);


    public async Task<string?> GetSubTokenPAsync(string sub) => await _db.StringGetAsync($"subtokenp:{sub}");


    public async Task SetTokenPTokenAsync(string tokenp, string token) => await _db.StringSetAsync($"tokenptoken:{tokenp}", token);


    public async Task<string?> GetTokenPTokenAsync(string tokenp) => await _db.StringGetAsync($"tokenptoken:{tokenp}");

    public async Task SetSubIdTokenAsync(string sub, string idToken) => await _db.StringSetAsync($"subidtoken:{sub}", idToken);


    public async Task<string?> GetSubIdTokenAsync(string sub) => await _db.StringGetAsync($"subidtoken:{sub}");


    public async Task SetTokenPRefreshTokenAsync(string tokenp, string refreshToken) => await _db.StringSetAsync($"tokenprefreshtoken:{tokenp}", refreshToken);


    public async Task<string?> GetTokenPRefreshTokenAsync(string tokenp) => await _db.StringGetAsync($"tokenprefreshtoken:{tokenp}");

}
