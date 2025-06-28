

using Duende.IdentityModel;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using PlainBridge.Api.ApiEndPoint.Endpoints;
using PlainBridge.Api.ApiEndPoint.ErrorHandling;
using PlainBridge.Api.Application.DTOs;

using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddOptions<ApplicationSetting>().Bind(builder.Configuration.GetSection("ApplicationSetting"));

builder.Services.AddApiProjectDatabase();
builder.Services.AddApiProjectServices();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.AddRedisClient(connectionName: "cache");
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

app.MapGroup("api/")
    .MapHostApplicationEndpoint();

app.MapGroup("api/")
    .MapServerApplicationEndpoint();

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



await app.RunAsync();


IServiceCollection AddAuthentication(this IServiceCollection services)
{

    var serviceProvider = services.BuildServiceProvider();
    var connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
    var appSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSetting>>();

    services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    object configuration = null;
                    if (!string.IsNullOrWhiteSpace(appSettings.Value.ZIRALINK_USE_HTTP) && bool.Parse(appSettings.Value.ZIRALINK_USE_HTTP!))
                    {
                        options.RequireHttpsMetadata = false;
                    }

                    options.Authority = new Uri(appSettings.Value.ZIRALINK_URL_IDS!).ToString();
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
                    if (!string.IsNullOrWhiteSpace(appSettings.Value.ZIRALINK_USE_HTTP) && bool.Parse(appSettings.Value.ZIRALINK_USE_HTTP!))
                    {
                        options.RequireHttpsMetadata = false;
                    }

                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = new Uri(appSettings.Value.ZIRALINK_URL_IDS!).ToString();
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