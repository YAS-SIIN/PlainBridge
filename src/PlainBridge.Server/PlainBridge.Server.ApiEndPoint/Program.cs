using System.Globalization; 
using PlainBridge.Server.ApiEndPoint; 
using PlainBridge.Server.Application.DTOs;
using Elastic.Serilog.Sinks;
using Serilog;

var simpleLogger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

simpleLogger.Information("Starting up");

try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    builder.AddServiceDefaults();

    builder.AddElasticsearchClient(connectionName: "elasticsearch");

    builder.Host.UseSerilog((ctx, services, lc) => lc
        .WriteTo.Elasticsearch()
        .Enrich.FromLogContext()
        .ReadFrom.Services(services), preserveStaticLogger: true);


    // Add services to the container.

    builder.Services.AddServerProjectServices();

    builder.AddRabbitMQClient(connectionName: "messaging");
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
