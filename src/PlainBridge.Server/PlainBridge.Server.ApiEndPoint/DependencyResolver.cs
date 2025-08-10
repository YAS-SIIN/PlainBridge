using PlainBridge.Server.ApiEndPoint.Middlewares;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.Server.Application.Services.ApiExternalBus;
using PlainBridge.Server.Application.Services.AppProjectConsumer;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.ServerBus;
using PlainBridge.Server.Application.Services.WebSocket;
using Serilog;

namespace PlainBridge.Server.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddServerProjectServices(this IServiceCollection services)
    {

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddMemoryCache();
        services.AddServerProjectServices();
        services.AddHostedService<Worker>();

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
