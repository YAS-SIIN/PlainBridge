
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using PlainBridge.Server.ApiEndPoint.ErrorHandling;
using PlainBridge.Server.ApiEndPoint.Middleware;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application;

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
        services.AddHostedService<Worker>();
        services.AddHttpServices();
        services.AddProblemDetails();

        services.AddServerApplicationProjectServices();
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
