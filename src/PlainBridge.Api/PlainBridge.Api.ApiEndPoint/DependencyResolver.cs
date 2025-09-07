using System.IdentityModel.Tokens.Jwt; 
using Duende.Bff.Yarp;
using Duende.IdentityModel; 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlainBridge.Api.ApiEndPoint.Abstractions;
using PlainBridge.Api.ApiEndPoint.ErrorHandling;
using PlainBridge.Api.Application;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Infrastructure;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.Persistence.Cache;
using Serilog; 

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
            options.MaximumPayloadBytes = appSettings.Value.HybridCacheMaximumPayloadBytes;
            options.MaximumKeyLength = appSettings.Value.HybridCacheMaximumKeyLength;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.None, 
                Expiration = TimeSpan.Parse(appSettings.Value.HybridDistributedCacheExpirationTime),
                LocalCacheExpiration = TimeSpan.Parse(appSettings.Value.HybridMemoryCacheExpirationTime)
            };
        });

        // Add services to the container 

        services.AddProblemDetails();
        services.AddOpenApi();

        services.AddAuthorization();

        services.AddApiApplicationProjectServices();
        services.AddApiInfrastructureProjectServices();

        services.AddAuthentication(appSettings.Value);
        services.AddExceptionHandler<ErrorHandler>();

         

        services.AddEndpoints();

        services.AddBff()
              .AddRemoteApis();
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddHttpContextAccessor();

        services.AddHttpServices();

         

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
                    var cacheManagement = context.HttpContext.RequestServices.GetRequiredService<ICacheManagement>();
                    var cancellationToken = context.HttpContext.RequestAborted;
                    var tokenp = context.Request.Headers["Authorization"];
                    tokenp = tokenp.ToString().Replace("Bearer ", "");
                    //var token = await tokenService.GetTokenPTokenAsync(tokenp.ToString()); 
                    var token = await cacheManagement.SetGetTokenPTokenAsync(tokenp.ToString(), cancellationToken: cancellationToken); 

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
