using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Test;

namespace PlainBridge.IdentityServer.EndPoint;

public static class TestUsers
{
    public static List<TestUser> Users
    {
        get
        {
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = "69118",
                country = "Germany"
            };

            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "Yasin",
                    Password = "Yasin",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Yasin Asadnezhad"),
                        new Claim(JwtClaimTypes.GivenName, "Yasin"),
                        new Claim(JwtClaimTypes.FamilyName, "Asadnezhad"),
                        new Claim(JwtClaimTypes.Email, "YasinAsadnezhad@example.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.example.com"),
                        new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "Sadeq",
                    Password = "Asadnezhad",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Sadeq Asadnezhad"),
                        new Claim(JwtClaimTypes.GivenName, "Sadeq"),
                        new Claim(JwtClaimTypes.FamilyName, "Asadnezhad"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@example.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.example.com"),
                        new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };
        }
    }
}
