using System.Globalization;
using Microsoft.Extensions.Options;
using PlainBridge.Server.ApiEndPoint;
using PlainBridge.Server.ApiEndPoint.Middlewares;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.WebSocketManagement;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", formatProvider: CultureInfo.InvariantCulture)
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddOptions<ApplicationSetting>().Bind(builder.Configuration.GetSection("ApplicationSetting"));  
builder.Services.AddMemoryCache();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddServerProjectServices();
//builder.Services.AddHostedService<Worker>();

// Configure Kestrel to support HTTP/3
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5002, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTP/3 requires HTTPS
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
    });
});

var app = builder.Build();

//app.Services.GetRequiredService<IWebSocketManagement>().();

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
 
await app.RunAsync();
