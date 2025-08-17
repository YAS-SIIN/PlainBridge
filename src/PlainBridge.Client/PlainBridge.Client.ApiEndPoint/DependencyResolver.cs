using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlainBridge.Client.Application.DTOs;
using PlainBridge.Client.Application.Handler.WebSocket;
using PlainBridge.Client.Application.Services.ClientBus;
using PlainBridge.Client.Application.Services.ServerBus;
using PlainBridge.Client.Application.Services.SharePortSocket;
using PlainBridge.Client.Application.Services.Signal;
using PlainBridge.Client.Application.Services.UsePortSocket;
using PlainBridge.Client.Application.Services.WebSocket;
using PlainBridge.Server.Application.Helpers.Http;

namespace PlainBridge.Client.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddClientProjectServices(this IServiceCollection services)
    {
        var appSettings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSettings>>();

        services.AddScoped<IWebSocketService, WebSocketService>();
        services.AddScoped<IServerBusService, ServerBusService>();
        services.AddScoped<IWebSocketHandler, WebSocketHandler>();
        services.AddScoped<IHttpHelper, HttpHelper>();
        services.AddScoped<IClientBusService, ClientBusService>();
        services.AddScoped<ISharePortSocketService, SharePortSocketService>();
        services.AddScoped<IUsePortSocketService, UsePortSocketService>();
        services.AddScoped<ISignalService, SignalService>();

        services.AddAuthentication(appSettings.Value);
        services.AddScoped<ClientWebSocket>(a =>
        {
            var webSocket = new ClientWebSocket();
            webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            webSocket.Options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            return webSocket;
        });


        services.AddHybridCache(options =>
        {
            // Maximum size of cached items
            options.MaximumPayloadBytes = appSettings.Value.HybridCacheMaximumPayloadBytes;
            options.MaximumKeyLength = appSettings.Value.HybridCacheMaximumKeyLength;

            // Default timeouts
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.Parse(appSettings.Value.HybridDistributedCacheExpirationTime),
                LocalCacheExpiration = TimeSpan.Parse(appSettings.Value.HybridMemoryCacheExpirationTime)
            };
        });

        services.AddAuthentication();
        return services;
    }


    public static IServiceCollection AddAuthentication(this IServiceCollection services, ApplicationSettings appSettings)
    {
        var serviceProvider = services.BuildServiceProvider();
        var hybridCache = serviceProvider.GetRequiredService<HybridCache>();

        // This is crucial - prevents the JWT handler from mapping 'sub' claim to ClaimTypes.NameIdentifier
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "oidc";
            options.DefaultSignOutScheme = "oidc";
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
          .AddOpenIdConnect("oidc", options =>
          {
              options.RequireHttpsMetadata = false;
              options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
              options.Authority = new Uri(appSettings.PlainBridgeIdsUrl).ToString();
              options.ClientId = appSettings.PlainBridgeIdsClientId;
              options.ClientSecret = appSettings.PlainBridgeIdsClientSecret ;
              options.ResponseType = OidcConstants.ResponseTypes.Code;
              options.Scope.Clear();
              options.Scope.Add("openid");
              options.Scope.Add("profile");
              options.Scope.Add("email");
              options.SaveTokens = true;

              options.GetClaimsFromUserInfoEndpoint = true;

          });

        return services;
    }

}
