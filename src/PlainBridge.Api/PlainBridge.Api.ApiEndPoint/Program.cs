

using System.Globalization;
using Duende.IdentityModel;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using PlainBridge.Api.ApiEndPoint.Endpoints;
using PlainBridge.Api.ApiEndPoint.ErrorHandling;
using PlainBridge.Api.Application.DTOs;
using Serilog;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", formatProvider: CultureInfo.InvariantCulture)
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(ctx.Configuration));

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));
var appSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSettings>>();

builder.Services.AddApiProjectDatabase();
builder.Services.AddApiProjectServices();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.AddRedisClient(connectionName: "cache");
builder.Services.AddAuthentication(appSettings.Value);
builder.Services.AddExceptionHandler<ErrorHandler>();

// Configure Kestrel to support HTTP/3
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTP/3 requires HTTPS
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
    });
});

var app = builder.Build();

app.MapGroup("")
    .MapLoginEndpoint();

app.MapGroup("api/")
    .MapHostApplicationEndpoint();

app.MapGroup("api/")
    .MapServerApplicationEndpoint();

app.MapGroup("api/")
    .MapUserEndpoint();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
;

app.UseCors("AllowCors");

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseBff();




await app.RunAsync();

public static class AuthenticationExtensions
{
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
}

