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
    IResourceBuilder<ElasticsearchResource> elasticsearch = default!;

    var identityserverEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint")
        .WithUrl("https://localhost:5003") 
        .PublishAsDockerFile();
     
    var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
        .WithUrl("https://localhost:5001")
        .WithReference(rabbitmq)
        .WithReference(cache) 
        .WithReference(identityserverEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq) 
        .WaitFor(identityserverEndpoint)
        .PublishAsDockerFile();


    var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
        .WithUrl("https://localhost:5002")
        .WithReference(cache)
        .WithReference(rabbitmq) 
        .WithReference(apiEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq) 
        .WaitFor(apiEndpoint)
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

    var clientEndpoint = builder.AddProject<Projects.PlainBridge_Client_ApiEndPoint>("client-apiendpoint")
    .WithUrl("https://localhost:5002")
    .WithReference(cache)
    .WithReference(rabbitmq) 
    .WithReference(serverEndpoint)
    .WaitFor(cache)
    .WaitFor(rabbitmq) 
    .WaitFor(serverEndpoint)
    .PublishAsDockerFile();


    if (!builder.Environment.IsDevelopment())
    {
        elasticsearch = builder.AddElasticsearch("elasticsearch");

        identityserverEndpoint
        .WithReference(elasticsearch)
        .WaitFor(elasticsearch);
         
        apiEndpoint
            .WithReference(elasticsearch)
            .WaitFor(elasticsearch);

        serverEndpoint
            .WithReference(elasticsearch)
            .WaitFor(elasticsearch);

        clientEndpoint
            .WithReference(elasticsearch)
            .WaitFor(elasticsearch);
         
    }


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
