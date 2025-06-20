 

using PlainBridge.Api.ApiEndPoint.Endpoints;
using PlainBridge.Api.ApiEndPoint.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
 
builder.Services.AddApiProjectDatabase();
builder.Services.AddApiProjectServices();
builder.AddRabbitMQClient(connectionName: "messaging");
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
 
app.MapDefaultEndpoints();

await app.RunAsync();
 