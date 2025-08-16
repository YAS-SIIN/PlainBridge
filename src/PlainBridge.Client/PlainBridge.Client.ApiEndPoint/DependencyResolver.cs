using System.Net.WebSockets;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
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

        services.AddScoped<ClientWebSocket>(a=>
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
        return services;
    }
}
