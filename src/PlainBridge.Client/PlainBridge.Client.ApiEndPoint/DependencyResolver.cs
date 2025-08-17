using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlainBridge.Client.ApiEndPoint.Endpoints;
using PlainBridge.Client.Application.DTOs;
using PlainBridge.Client.Application.Handler.WebSocket;
using PlainBridge.Client.Application.Services.ClientBus;
using PlainBridge.Client.Application.Services.ServerBus;
using PlainBridge.Client.Application.Services.SharePortSocket;
using PlainBridge.Client.Application.Services.Signal;
using PlainBridge.Client.Application.Services.UsePortSocket;
using PlainBridge.Client.Application.Services.WebSocket;
using PlainBridge.Server.Application.Helpers.Http;
using Serilog;

namespace PlainBridge.Client.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddClientProjectServices(this IServiceCollection services)
    {
        var appSettings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSettings>>();

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


        services.AddAuthentication();


        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;
            // Only loopback proxies are allowed by default.
            // Clear that restriction because forwarders are enabled by explicit
            // configuration.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });


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

    public static WebApplication AddUsers(this WebApplication app)
    {

        app.UseForwardedHeaders();
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        app.UseForwardedHeaders();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        
        app.UseRouting();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        } else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.MapGroup("api/")
            .MapLoginEndpoint();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors(); 

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapDefaultEndpoints();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseBff();

        return app;
    }
}
