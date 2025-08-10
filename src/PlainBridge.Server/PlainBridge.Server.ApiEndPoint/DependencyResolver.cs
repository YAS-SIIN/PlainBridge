 
using Serilog; 
using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;
using PlainBridge.Server.ApiEndPoint.ErrorHandling;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.ApiExternalBus;
using PlainBridge.Server.Application.Services.ServerBus;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.AppProjectConsumer;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.WebSocket;
using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.Server.ApiEndPoint.Middleware;
using PlainBridge.Server.Application.Services.ServerApplication;

namespace PlainBridge.Server.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddServerProjectServices(this IServiceCollection services)
    {

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddExceptionHandler<ErrorHandler>();
        services.AddMemoryCache(); 
        services.AddHostedService<Worker>();
        services.AddHttpServices();
        services.AddProblemDetails();

        services.AddScoped<IPlainBridgeApiClientHandler, PlainBridgeApiClientHandler>();

        services.AddScoped<IServerApplicationService, ServerApplicationService>(); 
        services.AddScoped<IHostApplicationService, HostApplicationService>();
        services.AddScoped<ICacheManagement, CacheManagement>();
        services.AddScoped<IApiExternalBusService, ApiExternalBusService>();
        services.AddScoped<IServerBusService, ServerBusService>();
        services.AddScoped<IServerApplicationConsumerService, ServerApplicationConsumerService>();
        services.AddScoped<IHttpRequestProxyService, HttpRequestProxyService>();
        services.AddScoped<IWebSocketService, WebSocketService>();
        services.AddScoped<Application.Services.Identity.IIdentityService, Application.Services.Identity.IdentityService>();
        services.AddScoped<ResponseCompletionSourcesManagement>();
        services.AddHttpServices();

        return services;
    }



    public static WebApplication AddUsers(this WebApplication app)
    {
         
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseWebSockets();
        app.UseMiddleware<HttpRequestProxyMiddleware>();
        app.UseMiddleware<WebSocketProxyMiddleware>();


        return app;
    }
}
