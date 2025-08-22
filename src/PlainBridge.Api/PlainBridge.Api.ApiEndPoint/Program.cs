
using System.Globalization;
using Elastic.Clients.Elasticsearch;
using Elastic.CommonSchema;
using Elastic.Ingest.Elasticsearch;
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


    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));


    builder.Services.AddApiProjectServices();

    // Configure Kestrel to support HTTP/3 only when not running in a container
    var inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!inContainer)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5001, listenOptions =>
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
catch (Exception ex) when (ex is not HostAbortedException)
{
    simpleLogger.Fatal(ex, "Unhandled exception");
}
finally
{
    simpleLogger.Information("Shut down complete"); 
}
