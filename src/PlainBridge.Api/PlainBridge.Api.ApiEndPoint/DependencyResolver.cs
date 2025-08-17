using System.IdentityModel.Tokens.Jwt;
using Duende.Bff.Yarp;
using Duende.IdentityModel;
using Elastic.CommonSchema;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlainBridge.Api.ApiEndPoint.Abstractions;
using PlainBridge.Api.ApiEndPoint.ErrorHandling;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.HostApplication;
using PlainBridge.Api.Application.Services.Identity;
using PlainBridge.Api.Application.Services.ServerApplication;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using Serilog;
using StackExchange.Redis;

namespace PlainBridge.Api.ApiEndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddApiProjectServices(this IServiceCollection services)
    {

        var appSettings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSettings>>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
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

        // Add services to the container.
        services.AddApiProjectDatabase();
        services.AddProblemDetails();
        services.AddOpenApi();
        services.AddAuthentication(appSettings.Value);
        services.AddExceptionHandler<ErrorHandler>();

        services.AddScoped<IHostApplicationService, HostApplicationService>();
        services.AddScoped<IServerApplicationService, ServerApplicationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IEventBus, RabbitMqEventBus>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ITokenService, TokenService>();
         

        services.AddAuthorization();
        services.AddEndpoints();

        services
              .AddBff()
              .AddRemoteApis();
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddHttpContextAccessor();

        services.AddHttpServices();



        return services;
    }

    public static IServiceCollection AddApiProjectDatabase(this IServiceCollection services)
    {
        services.AddDbContext<MainDbContext>(options => options.UseInMemoryDatabase("PlainBridgeApiDBContext"));

        return services;
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var assembly = typeof(IAssemblyMarker).Assembly;

        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }


    public static IServiceCollection AddAuthentication(this IServiceCollection services, ApplicationSettings appSettings)
    {
        var serviceProvider = services.BuildServiceProvider();
        var hybridCache = serviceProvider.GetRequiredService<HybridCache>();

        // This is crucial - prevents the JWT handler from mapping 'sub' claim to ClaimTypes.NameIdentifier
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "oidc";
            options.DefaultSignOutScheme = "oidc";
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddJwtBearer("Bearer", options =>
        {

            options.RequireHttpsMetadata = !appSettings.PlainBridgeUseHttp;


            options.Authority = new Uri(appSettings.PlainBridgeIdsUrl!).ToString();
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = async context =>
                {
                    var tokenp = context.Request.Headers["Authorization"];
                    tokenp = tokenp.ToString().Replace("Bearer ", "");
                    var token = await hybridCache.GetOrCreateAsync($"tokenptoken:{tokenp}", async ct => (string)default!);
                    context.Token = token;
                }
            };
        })
        .AddOpenIdConnect("oidc", options =>
        {
            options.RequireHttpsMetadata = !appSettings.PlainBridgeUseHttp;


            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Authority = new Uri(appSettings.PlainBridgeIdsUrl!).ToString();
            options.ClientId = "bff";
            options.ClientSecret = appSettings.PlainBridgeIdsClientSecret;
            options.ResponseType = OidcConstants.ResponseTypes.Code;
            options.Scope.Clear();
            options.Scope.Add("PlainBridge");
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("offline_access");
            options.SaveTokens = true;

            options.GetClaimsFromUserInfoEndpoint = true;
        });

        return services;
    }

    public static WebApplication AddUsers(this WebApplication app)
    {

        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors();
        app.MapEndpoints();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapDefaultEndpoints();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseBff();

        return app;
    }
    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        IEnumerable<IEndpoint> endpoints = app.Services
                                              .GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
