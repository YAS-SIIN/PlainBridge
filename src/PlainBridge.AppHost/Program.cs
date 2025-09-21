using Aspire.Hosting.Docker.Resources.ComposeNodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PlainBridge.AppHost.DTOs;
using Serilog;
using System.Globalization; 


var simpleLogger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

simpleLogger.Information("Starting up");


try
{
    var builder = DistributedApplication.CreateBuilder(args);

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    var appSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSettings>>();

    var dc = builder.AddDockerComposeEnvironment("plain-bridge");

    var cache = builder.AddRedis("cache")
        .WithRedisInsight();

    var rabbitmq = builder.AddRabbitMQ("messaging")
        .WithManagementPlugin();
    IResourceBuilder<ElasticsearchResource> elasticsearch = default!;

    var identityServerEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint")
        .WithUrl($"https://localhost:{appSettings.Value.PlainBridgeIdsPort.ToString()}")
        .WithEnvironment("IDS_PROJECT_PORT", appSettings.Value.PlainBridgeIdsPort.ToString());

    var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
        .WithUrl($"https://localhost:{appSettings.Value.PlainBridgeApiPort.ToString()}")
        .WithReference(rabbitmq)
        .WithReference(cache)
        .WithReference(identityServerEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(identityServerEndpoint)
        .WithEnvironment("API_PROJECT_PORT", appSettings.Value.PlainBridgeApiPort.ToString());

    var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
        .WithUrl($"https://localhost:{appSettings.Value.PlainBridgeServerPort.ToString()}")
        .WithReference(cache)
        .WithReference(rabbitmq)
        .WithReference(apiEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(apiEndpoint)
        .WithEnvironment("SERVER_PROJECT_PORT", appSettings.Value.PlainBridgeServerPort.ToString());


    var angularWebUi = builder.AddNpmApp("angular-webui", "../PlainBridge.Web/PlainBridge.Web.UI")
        .WithHttpEndpoint(port: appSettings.Value.PlainBridgeWebPort, env: "PORT")
        .WithReference(apiEndpoint)
        .WithReference(identityServerEndpoint)
        .WaitFor(identityServerEndpoint)
        .WaitFor(apiEndpoint)
        .WithExternalHttpEndpoints()
        .PublishAsDockerFile();


    // Fix: Convert int to string before interpolating into WithUrl to avoid CS0315
    var clientEndpoint = builder.AddProject<Projects.PlainBridge_Client_ApiEndPoint>("client-endpoint")
        .WithUrl($"https://localhost:{appSettings.Value.PlainBridgeClientPort.ToString()}")
        .WithReference(cache)
        .WithReference(rabbitmq)
        .WithReference(serverEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(serverEndpoint)
        .WithEnvironment("CLIENT_PROJECT_PORT", appSettings.Value.PlainBridgeClientPort.ToString());




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
 