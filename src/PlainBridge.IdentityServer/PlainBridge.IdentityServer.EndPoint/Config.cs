using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using PlainBridge.IdentityServer.EndPoint.DTOs;

namespace PlainBridge.IdentityServer.EndPoint;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("PlainBridge"),
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName) ,
        };

    public static IEnumerable<Client> Clients(ApplicationSettings applcationSettings) =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                // where to redirect to after login
                RedirectUris = { $"{applcationSettings.PlainBridgeWebUrl}/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { $"{applcationSettings.PlainBridgeWebUrl}/signout-callback-oidc" },

                AllowedScopes = {
                    "PlainBridge", 
                    IdentityServerConstants.StandardScopes.OpenId, 
                    IdentityServerConstants.LocalApi.ScopeName, 
                    IdentityServerConstants.StandardScopes.Profile, 
                    IdentityServerConstants.StandardScopes.Email,  
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                }
            },
             
            new Client
            {
                ClientId = "back",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

           
                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // scopes that client has access to
                AllowedScopes = { "PlainBridge", IdentityServerConstants.LocalApi.ScopeName, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.Email }
            },

            // BFF client for Backend for Frontend pattern
            new Client
            {
                ClientId = "bff",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { $"{applcationSettings.PlainBridgeApiUrl}/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { $"{applcationSettings.PlainBridgeApiUrl}/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.LocalApi.ScopeName,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "PlainBridge"
                },

                AllowOfflineAccess = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Absolute
            },
        };
}
