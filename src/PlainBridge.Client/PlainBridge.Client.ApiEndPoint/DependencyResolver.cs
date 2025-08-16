using System.Net.WebSockets;
using PlainBridge.Client.Application.Handler.WebSocket;
using PlainBridge.Client.Application.Services.ClientBus;
using PlainBridge.Client.Application.Services.ServerBus;
using PlainBridge.Client.Application.Services.SharePortSocket;
using PlainBridge.Client.Application.Services.UsePortSocket;
using PlainBridge.Client.Application.Services.WebSocket;
using PlainBridge.Server.Application.Helpers.Http;

namespace PlainBridge.Client.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddClientProjectServices(this IServiceCollection services)
    {
        services.AddScoped<IWebSocketService, WebSocketService>();
        services.AddScoped<IServerBusService, ServerBusService>();
        services.AddScoped<IWebSocketHandler, WebSocketHandler>();
        services.AddScoped<IHttpHelper, HttpHelper>();
        services.AddScoped<IClientBusService, ClientBusService>();
        services.AddScoped<ISharePortSocketService, SharePortSocketService>();
        services.AddScoped<IUsePortSocketService, UsePortSocketService>();

        services.AddScoped<ClientWebSocket>(a=>
        {
            var webSocket = new ClientWebSocket();
            webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            webSocket.Options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            return webSocket;
        });
        return services;
    }
}
