using System.IdentityModel.Tokens.Jwt;
using Duende.Bff.Yarp;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

    public static IServiceCollection AddAuthentication(this IServiceCollection services, ApplicationSettings appSettings)
    {
        var serviceProvider = services.BuildServiceProvider();
        var connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>(); 

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
            if (appSettings.PlainBridgeUseHttp)
            {
                options.RequireHttpsMetadata = false;
            }

            options.Authority = new Uri(appSettings.PlainBridgeIdsUrl!).ToString();
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };

            var db = connectionMultiplexer.GetDatabase(10);

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = async context =>
                {
                    var tokenp = context.Request.Headers["Authorization"];
                    tokenp = tokenp.ToString().Replace("Bearer ", "");
                    var token = await db.StringGetAsync($"tokenptoken:{tokenp}");
                    context.Token = token;
                }
            };
        })
        .AddOpenIdConnect("oidc", options =>
        {
            if (appSettings.PlainBridgeUseHttp)
            {
                options.RequireHttpsMetadata = false;
            }

            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Authority = new Uri(appSettings.PlainBridgeIdsUrl!).ToString();
            options.ClientId = "bff";
            options.ClientSecret = "secret";
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

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapDefaultEndpoints();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseBff();

        return app;
    }
}
