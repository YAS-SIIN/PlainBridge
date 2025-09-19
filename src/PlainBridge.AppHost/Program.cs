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

    var dc = builder.AddDockerComposeEnvironment("plain-bridge");

    var cache = builder.AddRedis("cache")
        .WithRedisInsight();

    var rabbitmq = builder.AddRabbitMQ("messaging")
        .WithManagementPlugin();
    IResourceBuilder<ElasticsearchResource> elasticsearch = default!;

    var identityServerEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint")
        .WithUrl("https://localhost:5003");

    var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
        .WithUrl("https://localhost:5001")
        .WithReference(rabbitmq)
        .WithReference(cache)
        .WithReference(identityServerEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(identityServerEndpoint);


    var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
        .WithUrl("https://localhost:5002")
        .WithReference(cache)
        .WithReference(rabbitmq)
        .WithReference(apiEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(apiEndpoint);


    var angularWebUi = builder.AddNpmApp("angular-webui", "../PlainBridge.Web/PlainBridge.Web.UI")
        .WithHttpEndpoint(port: 5004, env: "PORT")
        .WithReference(apiEndpoint)
        .WithReference(identityServerEndpoint)
        .WaitFor(identityServerEndpoint)
        .WaitFor(apiEndpoint)
        .WithExternalHttpEndpoints()
        .PublishAsDockerFile();


    var clientEndpoint = builder.AddProject<Projects.PlainBridge_Client_ApiEndPoint>("client-endpoint")
        .WithUrl("https://localhost:5005")
        .WithReference(cache)
        .WithReference(rabbitmq)
        .WithReference(serverEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(serverEndpoint);




    if (!builder.Environment.IsDevelopment())
    {
        elasticsearch = builder.AddElasticsearch("elasticsearch");

        identityServerEndpoint
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

        // Suppress ASPIRECOMPUTE001 diagnostic for WithComputeEnvironment usage
#pragma warning disable ASPIRECOMPUTE001
        clientEndpoint.WithComputeEnvironment(dc);

    }
    // Suppress ASPIRECOMPUTE001 diagnostic for WithComputeEnvironment usage
#pragma warning disable ASPIRECOMPUTE001
    identityServerEndpoint.WithComputeEnvironment(dc);
    cache.WithComputeEnvironment(dc);
    rabbitmq.WithComputeEnvironment(dc);
    apiEndpoint.WithComputeEnvironment(dc);
    serverEndpoint.WithComputeEnvironment(dc);
    clientEndpoint.WithComputeEnvironment(dc);

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
 