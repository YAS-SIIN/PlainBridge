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


    var env = builder.AddDockerComposeEnvironment("env");

    var cache = builder.AddRedis("cache") .WithRedisInsight();
    var rabbitmq = builder.AddRabbitMQ("messaging").WithManagementPlugin();
    IResourceBuilder<ElasticsearchResource> elasticsearch = default!;

    var identityserverEndpoint = builder
        .AddDockerfile("identityserver-endpoint", "..\\..", "src/PlainBridge.IdentityServer/PlainBridge.IdentityServer.EndPoint/Dockerfile")
        .WithUrl("https://localhost:5003");
     
    var apiEndpoint = builder
        .AddDockerfile("api-endpoint", "..\\..", "src/PlainBridge.Api/PlainBridge.Api.ApiEndPoint/Dockerfile")
        .WithUrl("https://localhost:5001")
        .WithReference(rabbitmq)
        .WithReference(cache)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(identityserverEndpoint);


    var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
        .WithUrl("https://localhost:5002")
        .WithReference(cache)
        .WithReference(rabbitmq) 
        .WithReference(apiEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq) 
        .WaitFor(apiEndpoint)
        .PublishAsDockerFile();

    var angularWebUi = builder
        .AddDockerfile("angular-web", "..\\..", "src/PlainBridge.Web/PlainBridge.Web.UI/Dockerfile")
        .WithHttpEndpoint(port: 5004)
        .WaitFor(identityserverEndpoint)
        .WaitFor(apiEndpoint);

    var clientEndpoint = builder
    .AddDockerfile("client-apiendpoint", "..\\..", "src/PlainBridge.Client/PlainBridge.Client.ApiEndPoint/Dockerfile")
    .WithUrl("https://localhost:5005")
    .WithReference(cache)
    .WithReference(rabbitmq)
    .WaitFor(cache)
    .WaitFor(rabbitmq)
    .WaitFor(serverEndpoint);
);


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
