

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.Server.Application.Services.ApiExternalBus;
using PlainBridge.Server.Application.Services.AppProjectConsumer;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.Identity;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using PlainBridge.Server.Application.Services.ServerApplication;
using PlainBridge.Server.Application.Services.ServerBus;
using PlainBridge.Server.Application.Services.WebSocket;

namespace PlainBridge.Server.Application;

public static class DependencyResolver
{
    public static IServiceCollection AddServerApplicationProjectServices(this IServiceCollection services)
    {

         
        services.AddSingleton<IPlainBridgeApiClientService, PlainBridgeApiClientService>();
        services.AddSingleton<IServerApplicationService, ServerApplicationService>();
        services.AddSingleton<IHostApplicationService, HostApplicationService>();
        services.AddSingleton<ICacheManagement, CacheManagement>();
        services.AddSingleton<IApiExternalBusService, ApiExternalBusService>();
        services.AddSingleton<IServerBusService, ServerBusService>();
        services.AddSingleton<IServerApplicationConsumerService, ServerApplicationConsumerService>();
        services.AddSingleton<IHttpRequestProxyService, HttpRequestProxyService>();
        services.AddSingleton<IWebSocketService, WebSocketService>();
        services.AddSingleton<IIdentityService, IdentityService>();
        services.AddSingleton<ResponseCompletionSourcesManagement>();
  
        return services;
    }


     
}
