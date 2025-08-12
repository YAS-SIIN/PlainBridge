using System.Globalization;
using Microsoft.Extensions.Hosting;
using Serilog;


var simpleLogger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

simpleLogger.Information("Starting up");

try
{
    var builder = DistributedApplication.CreateBuilder(args);

    var cache = builder.AddRedis("cache");
    var rabbitmq = builder.AddRabbitMQ("messaging").WithManagementPlugin();
    var elasticsearch = builder.AddElasticsearch("elasticsearch");

    var identityserverEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint")
        .WithUrl("https://localhost:5003")
        .WithReference(elasticsearch)
        .WaitFor(elasticsearch)
        .PublishAsDockerFile();

    var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
        .WithUrl("https://localhost:5001")
        .WithReference(identityserverEndpoint)
        .WithReference(rabbitmq)
        .WithReference(cache)
        .WithReference(elasticsearch)
        .WaitFor(identityserverEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(elasticsearch)
        .PublishAsDockerFile();


    var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
        .WithUrl("https://localhost:5002")
        .WithReference(apiEndpoint)
        .WithReference(rabbitmq)
        .WithReference(elasticsearch)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(elasticsearch)
        .PublishAsDockerFile();

    var angularWebUi =
    builder.AddNpmApp("angularWebUi", "../PlainBridge.Web/PlainBridge.Web.UI")
        .WithHttpEndpoint(port: 5004, env: "PORT")
        .WithReference(apiEndpoint)
        .WithReference(identityserverEndpoint)
        .WaitFor(identityserverEndpoint)
        .WaitFor(apiEndpoint)
        .WithExternalHttpEndpoints()
        .PublishAsDockerFile();

    //builder.AddDockerfile("PlainBridge-AppHost", "relative/context/path")
    //    .WithReference(apiEndpoint)
    //    .WithReference(identityserverEndpoint)
    //    .WithReference(serverEndpoint)
    //    .WithReference(angularWebUi);   

    await builder.Build().RunAsync();

}
catch (Exception ex) when (ex is not HostAbortedException)
{
    simpleLogger.Fatal(ex, "Unhandled exception");
}
finally
{
    simpleLogger.Information("Shut down complete");
}
