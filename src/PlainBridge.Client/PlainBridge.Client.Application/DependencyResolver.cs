

using Microsoft.Extensions.DependencyInjection;
using PlainBridge.Client.Application.Handler.HttpRequest;
using PlainBridge.Client.Application.Handler.WebSocket;
using PlainBridge.Client.Application.Helpers.Http;
using PlainBridge.Client.Application.Management.Cache;
using PlainBridge.Client.Application.Management.WebSocket;
using PlainBridge.Client.Application.Services.ClientBus;
using PlainBridge.Client.Application.Services.ServerBus;
using PlainBridge.Client.Application.Services.SharePortSocket;
using PlainBridge.Client.Application.Services.Signal;
using PlainBridge.Client.Application.Services.UsePortSocket;
using PlainBridge.Client.Application.Services.WebSocket;

namespace PlainBridge.Client.Application;

public static class DependencyResolver
{
    public static IServiceCollection AddClientApplicationProjectServices(this IServiceCollection services)
    {

        services.AddSingleton<IHttpHelper, HttpHelper>();
        services.AddSingleton<ICacheManagement, CacheManagement>();
        services.AddSingleton<IWebSocketManagement, WebSocketManagement>();
        services.AddSingleton<IWebSocketService, WebSocketService>();
        services.AddSingleton<IServerBusService, ServerBusService>();
        services.AddSingleton<IHttpRequestHandler, HttpRequestHandler>();
        services.AddSingleton<IWebSocketHandler, WebSocketHandler>();
        services.AddSingleton<ISharePortSocketService, SharePortSocketService>();
        services.AddSingleton<IUsePortSocketService, UsePortSocketService>();
        services.AddSingleton<IClientBusService, ClientBusService>();
        services.AddSingleton<ISignalService, SignalService>();


        return services;
    }


}
