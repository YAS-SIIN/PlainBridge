
using System.Globalization;
using PlainBridge.Api.ApiEndPoint.Endpoints;
using PlainBridge.Api.Application.DTOs;
using Serilog;
using PlainBridge.Api.ApiEndPoint;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{


    var builder = WebApplication.CreateBuilder(args);

    var log = builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", formatProvider: CultureInfo.InvariantCulture)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));


    // Add service defaults & Aspire client integrations.
    builder.AddServiceDefaults();

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    builder.Services.AddApiProjectDatabase();

    builder.AddRabbitMQClient(connectionName: "messaging");
    builder.AddRedisClient(connectionName: "cache");

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



    app.MapGroup("").MapLoginEndpoint();
    app.MapGroup("api/").MapHostApplicationEndpoint();
    app.MapGroup("api/").MapServerApplicationEndpoint();
    app.MapGroup("api/").MapUserEndpoint();


    app.AddUsers();

    await app.RunAsync();


}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
