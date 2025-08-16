using System.Net.WebSockets;
using PlainBridge.Client.Application.Services.WebSocket;

namespace PlainBridge.Client.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddClientProjectServices(this IServiceCollection services)
    {
        services.AddScoped<IWebSocketService, WebSocketService>();

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
