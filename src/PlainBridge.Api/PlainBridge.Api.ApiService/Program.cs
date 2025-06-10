// Pseudocode:
// 1. Configure Kestrel to support HTTP/3 in the WebApplication builder.
// 2. Set up endpoints and services as before.
// 3. Ensure the server listens on a protocol supporting HTTP/3 (e.g., HTTPS with ALPN).
// 4. Optionally, set up TLS certificate for HTTP/3 (required).
// 5. Add the Kestrel configuration in builder.WebHost.ConfigureKestrel.

using PlainBridge.Api.ApiService.Endpoints;
using PlainBridge.Api.ApiService.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddDatabase();

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

app.MapGroup("api/v1")
    .MapHostApplicationEndpoint();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
