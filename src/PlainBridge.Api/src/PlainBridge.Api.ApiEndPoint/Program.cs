
using PlainBridge.Api.ApiEndPoint;
using PlainBridge.Api.Infrastructure.DTOs;
try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.AddServiceDefaults();

    builder.AddRedisClient(connectionName: "cache");
    builder.AddRedisDistributedCache(connectionName: "cache");
    builder.AddRabbitMQClient(connectionName: "messaging");

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    builder.Services.AddApiProjectServices();

    // Configure Kestrel to support HTTP/3 only when not running in a container
    var inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!inContainer)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("API_PROJECT_PORT") ?? "5051"), listenOptions =>
            {
                listenOptions.UseHttps(); // HTTP/3 requires HTTPS
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
            });
        });
    }

    var app = builder.Build();

    app.AddUsers();

    await app.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
}