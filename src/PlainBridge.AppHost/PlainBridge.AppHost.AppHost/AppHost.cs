using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using PlainBridge.AppHost.DTOs;
 
try
{


    var builder = DistributedApplication.CreateBuilder(args);

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    //var appSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSettings>>();

    var dc = builder.AddDockerComposeEnvironment("plain-bridge");

    var cache = builder.AddRedis("cache")
        .WithRedisInsight()
        .WithComputeEnvironment(dc);

    var rabbitmq = builder.AddRabbitMQ("messaging")
        .WithManagementPlugin()
        .WithComputeEnvironment(dc);


    var identityServerEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint")
        .WithIconName("ShieldKeyhole")
        .WithComputeEnvironment(dc);

    var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
        .WithIconName("Link")
        .WithReference(rabbitmq)
        .WithReference(cache)
        .WithReference(identityServerEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(identityServerEndpoint)
        .WithComputeEnvironment(dc);

    var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
        .WithIconName("server")
        .WithReference(cache)
        .WithReference(rabbitmq)
        .WithReference(apiEndpoint)
        .WithReference(apiEndpoint)
        .WithReference(identityServerEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(apiEndpoint)
        .WaitFor(identityServerEndpoint)
        .WaitFor(apiEndpoint)
        .WithComputeEnvironment(dc);


    var angularProjectPath = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, "../../PlainBridge.Web/src/PlainBridge.Web.UI"));
    var angularWebUi = builder.AddJavaScriptApp("angular-webui", angularProjectPath)
        .WithIconName("globe")
        .WithRunScript("start")
        .WithHttpEndpoint(env: "PORT")
        .WithNpm(install: false)
        .WithReference(apiEndpoint)
        .WithReference(identityServerEndpoint)
        .WaitFor(identityServerEndpoint)
        .WaitFor(apiEndpoint)
        .WithExternalHttpEndpoints()
        .PublishAsDockerFile()
        .WithComputeEnvironment(dc);


    // Fix: Convert int to string before interpolating into WithUrl to avoid CS0315
    var clientEndpoint = builder.AddProject<Projects.PlainBridge_Client_ApiEndPoint>("client-endpoint")
        .WithIconName("layout-dashboard")
        .WithReference(cache)
        .WithReference(rabbitmq)
        .WithReference(serverEndpoint)
        .WaitFor(cache)
        .WaitFor(rabbitmq)
        .WaitFor(serverEndpoint)
        .WithComputeEnvironment(dc);


    //var elasticsearchPassword = builder.AddParameter("elasticsearch-password", value: appSettings.Value.ElasticSearchPassword, secret: true);

    //var elasticsearch = builder.AddElasticsearch("elasticsearch", password: elasticsearchPassword)
    //    .WithDataVolume()
    //    .WithLifetime(ContainerLifetime.Persistent)
    //    .WithComputeEnvironment(dc);

    //var collector = builder.AddOpenTelemetryCollector("otel-collector")
    //    .WithConfig("otel-collector-config.yaml")
    //    .WithEnvironment("ELASTICSEARCH_URL", elasticsearch.GetEndpoint("http"))
    //    .WithEnvironment("ELASTICSEARCH_USERNAME", "elastic")
    //    .WithEnvironment("ELASTICSEARCH_PASSWORD", elasticsearchPassword)
    //    .WithReference(elasticsearch)
    //    .WaitFor(elasticsearch)
    //    .WithAppForwarding()
    //    .WithComputeEnvironment(dc);


    await builder.Build().RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
    throw;
}
