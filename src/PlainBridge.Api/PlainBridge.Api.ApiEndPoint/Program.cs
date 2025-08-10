
using System.Globalization;
using Elastic.Clients.Elasticsearch;
using Elastic.Serilog.Sinks;
using PlainBridge.Api.ApiEndPoint;
using PlainBridge.Api.ApiEndPoint.Endpoints;
using PlainBridge.Api.Application.DTOs;
using Serilog; 


var simpleLogger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

simpleLogger.Information("Starting up");


try
{ 
    var builder = WebApplication.CreateBuilder(args);
     
    builder.AddServiceDefaults();

    builder.AddRabbitMQClient(connectionName: "messaging");
    builder.AddRedisClient(connectionName: "cache");
    builder.AddElasticsearchClient(connectionName: "elasticsearch");
     
    builder.Host.UseSerilog((ctx, services, lc) => lc
        .WriteTo.Elasticsearch()
        .Enrich.FromLogContext() 
        .ReadFrom.Services(services), preserveStaticLogger: true);

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));


    builder.Services.AddApiProjectServices();

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
    simpleLogger.Fatal(ex, "Unhandled exception");
}
finally
{
    simpleLogger.Information("Shut down complete"); 
}
