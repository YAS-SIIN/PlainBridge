using System.Globalization;
using Elastic.Clients.Elasticsearch;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using PlainBridge.Server.ApiEndPoint;
using PlainBridge.Server.Application.DTOs;
using Serilog;
using Microsoft.AspNetCore.TestHost; // Add this using directive at the top

var simpleLogger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

simpleLogger.Information("Starting up");

try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    builder.AddServiceDefaults();

    builder.AddRedisClient(connectionName: "cache");
    builder.AddRedisDistributedCache(connectionName: "cache");
    builder.AddRabbitMQClient(connectionName: "messaging");
    builder.AddElasticsearchClient(connectionName: "elasticsearch");

    builder.Host.UseSerilog((ctx, services, lc) =>
    {
        lc.ReadFrom.Services(services)
          .WriteTo.Console(
              outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
              formatProvider: CultureInfo.InvariantCulture);

        if (!ctx.HostingEnvironment.IsDevelopment())
        {
            var esClient = services.GetRequiredService<ElasticsearchClient>();
            var elasticConfig = new ElasticsearchSinkOptions(esClient.Transport) { BootstrapMethod = BootstrapMethod.Failure };

            lc.WriteTo.Elasticsearch(elasticConfig);
        }
    }, preserveStaticLogger: true);


    builder.Services.AddServerProjectServices();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("SERVER_PROJECT_PORT") ?? "5002"), listenOptions =>
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


public partial class Program { }