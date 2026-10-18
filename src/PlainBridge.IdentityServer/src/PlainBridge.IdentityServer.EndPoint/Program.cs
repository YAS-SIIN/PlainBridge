using PlainBridge.IdentityServer.EndPoint;
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.IdentityServer.EndPoint.Endpoints;
try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.AddServiceDefaults();

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    builder.Services.AddIDSProjectServices();

    // Configure Kestrel to support HTTP/3 only when not running in a container
    var inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!inContainer)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("IDS_PROJECT_PORT") ?? "5053"), listenOptions =>
            {
                listenOptions.UseHttps(); // HTTP/3 requires HTTPS
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
            });
        });
    }

    var app = builder.Build();

    app.MapGroup("api/")
        .MapUserEndpoint();

    app.AddUsers();
    await app.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
}

public partial class Program { }