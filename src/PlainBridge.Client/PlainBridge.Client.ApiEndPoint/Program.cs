
using System.Globalization;
using Elastic.Clients.Elasticsearch;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.TestHost;
using PlainBridge.Client.ApiEndPoint;
using PlainBridge.Client.Application.DTOs;
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
            var elasticConfig = new ElasticsearchSinkOptions(esClient.Transport) { BootstrapMethod = Elastic.Ingest.Elasticsearch.BootstrapMethod.Failure };

            lc.WriteTo.Elasticsearch(elasticConfig);
        }
    }, preserveStaticLogger: true);


    builder.Services.AddClientProjectServices();
     

  
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("CLIENT_PROJECT_PORT") ?? "5005"), listenOptions =>
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
