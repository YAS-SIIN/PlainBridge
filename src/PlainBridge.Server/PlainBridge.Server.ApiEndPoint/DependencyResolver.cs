 
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using PlainBridge.Server.ApiEndPoint.ErrorHandling;
using PlainBridge.Server.ApiEndPoint.Middleware;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.Server.Application.Services.ApiExternalBus;
using PlainBridge.Server.Application.Services.AppProjectConsumer;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.ServerApplication;
using PlainBridge.Server.Application.Services.ServerBus;
using PlainBridge.Server.Application.Services.WebSocket;
using Serilog; 

namespace PlainBridge.Server.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddServerProjectServices(this IServiceCollection services)
    {

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
