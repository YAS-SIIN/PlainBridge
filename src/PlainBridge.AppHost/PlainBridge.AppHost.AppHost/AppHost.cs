using Aspire.Hosting.Docker.Resources.ComposeNodes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using PlainBridge.AppHost.DTOs;

using System.Globalization;
try
{


    var builder = DistributedApplication.CreateBuilder(args);

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    var appSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSettings>>();

    var dc = builder.AddDockerComposeEnvironment("plain-bridge");

    var cache = builder.AddRedis("cache")
        .WithRedisInsight()
        .WithComputeEnvironment(dc);

    var rabbitmq = builder.AddRabbitMQ("messaging")
        .WithManagementPlugin()
        .WithComputeEnvironment(dc);


    var identityServerEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint")
        .WithUrl($"https://localhost:{appSettings.Value.PlainBridgeIdsPort.ToString()}")
        .WithEnvironment("IDS_PROJECT_PORT", appSettings.Value.PlainBridgeIdsPort.ToString())
        .WithComputeEnvironment(dc);

    var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
        .WithUrl($"https://localhost:{appSettings.Value.PlainBridgeApiPort.ToString()}")
        .WithReference(rabbitmq)
        .WithReference(cache)
        .WithReference(identityServerEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(identityServerEndpoint)
        .WithEnvironment("API_PROJECT_PORT", appSettings.Value.PlainBridgeApiPort.ToString())
        .WithComputeEnvironment(dc);

    var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
        .WithUrl($"https://localhost:{appSettings.Value.PlainBridgeServerPort.ToString()}")
        .WithReference(cache)
        .WithReference(rabbitmq)
        .WithReference(apiEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(apiEndpoint)
        .WithEnvironment("SERVER_PROJECT_PORT", appSettings.Value.PlainBridgeServerPort.ToString())
        .WithComputeEnvironment(dc);


    var angularWebUi = builder.AddJavaScriptApp("angular-webui", "../PlainBridge.Web/src/PlainBridge.Web.UI")
        .WithRunScript("start")
        .WithHttpEndpoint(port: appSettings.Value.PlainBridgeWebPort, env: "PORT")
        .WithNpm(install: false) // Set to true if you want to use pnpm instead of npm
        .WithReference(apiEndpoint)
        .WithReference(identityServerEndpoint)
        .WaitFor(identityServerEndpoint)
        .WaitFor(apiEndpoint)
        .WithExternalHttpEndpoints()
        .PublishAsDockerFile()
        .WithComputeEnvironment(dc);


    // Fix: Convert int to string before interpolating into WithUrl to avoid CS0315
    var clientEndpoint = builder.AddProject<Projects.PlainBridge_Client_ApiEndPoint>("client-endpoint")
        .WithUrl($"https://localhost:{appSettings.Value.PlainBridgeClientPort.ToString()}")
        .WithReference(cache)
        .WithReference(rabbitmq)
        .WithReference(serverEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(serverEndpoint)
        .WithEnvironment("CLIENT_PROJECT_PORT", appSettings.Value.PlainBridgeClientPort.ToString())
        .WithComputeEnvironment(dc);


    var elasticsearchPassword = builder.AddParameter("elasticsearch-password", value: appSettings.Value.ElasticSearchPassword, secret: true);
    
    var elasticsearch = builder.AddElasticsearch("elasticsearch", password: elasticsearchPassword, 5090)
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent)
        .WithComputeEnvironment(dc);

    var collector = builder.AddOpenTelemetryCollector("otel-collector")
        .WithConfig("otel-collector-config.yaml")
        .WithEnvironment("ELASTICSEARCH_URL", elasticsearch.GetEndpoint("http"))
        .WithEnvironment("ELASTICSEARCH_USERNAME", "elastic")
        .WithEnvironment("ELASTICSEARCH_PASSWORD", elasticsearchPassword)
        .WithReference(elasticsearch)
        .WaitFor(elasticsearch)
        .WithAppForwarding();

    //builder.AddDockerfile("PlainBridge-AppHost", "relative/context/path")
    //    .WithReference(apiEndpoint)
    //    .WithReference(identityServerEndpoint)
    //    .WithReference(serverEndpoint)
    //    .WithReference(angularWebUi);

    await builder.Build().RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
}