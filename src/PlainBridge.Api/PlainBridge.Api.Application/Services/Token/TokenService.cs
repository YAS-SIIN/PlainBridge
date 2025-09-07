
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


      


}
