using Microsoft.Extensions.Options;
using PlainBridge.Server.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddServerProjectServices();

builder.Services.Configure<ApplicationSetting>(builder.Configuration);
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<ApplicationSetting>>().Value);
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

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
