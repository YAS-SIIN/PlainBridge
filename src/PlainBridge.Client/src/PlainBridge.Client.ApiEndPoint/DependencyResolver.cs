using System.Net.WebSockets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Hybrid;
using PlainBridge.Client.ApiEndPoint.ErrorHandling;
using PlainBridge.Client.Application.DTOs;
using PlainBridge.Client.Application;


namespace PlainBridge.Client.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddClientProjectServices(this IServiceCollection services, IConfiguration configuration)
    { 
        var appSettings = configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>() ?? throw new InvalidOperationException("ApplicationSettings configuration is missing.");

        services.AddHybridCache(options =>
        {
            // Maximum size of cached items
            options.MaximumPayloadBytes = appSettings.HybridCacheMaximumPayloadBytes;
            options.MaximumKeyLength = appSettings.HybridCacheMaximumKeyLength;

            // Default timeouts
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.Parse(appSettings.HybridDistributedCacheExpirationTime),
                LocalCacheExpiration = TimeSpan.Parse(appSettings.HybridMemoryCacheExpirationTime)
            };
        });

        services.AddExceptionHandler<ErrorHandler>();
        services.AddOpenApi();

        services.AddSingleton<ClientWebSocket>(a =>
        {
            var webSocket = new ClientWebSocket();
            webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            //webSocket.Options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            return webSocket;
        });

        services.AddHttpContextAccessor();
        services.AddProblemDetails();
        services.AddClientApplicationProjectServices();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;
            // Only loopback proxies are allowed by default.
            // Clear that restriction because forwarders are enabled by explicit configuration.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddHostedService<Worker>();

        return services;
    }
     
    public static WebApplication AddUsers(this WebApplication app)
    {

        app.UseExceptionHandler();

        app.UseForwardedHeaders();
         
        app.UseRouting();

        // Configure the HTTP request pipeline. 
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        else
        {
            //app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.MapDefaultEndpoints();

        return app;
    }
}
