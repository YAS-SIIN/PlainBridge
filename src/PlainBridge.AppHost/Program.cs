var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var rabbitmq = builder.AddRabbitMQ("messaging").WithManagementPlugin();

var identityserverEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint")
    .WithUrl("https://localhost:5003");

var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
    .WithUrl("https://localhost:5001")
    .WithReference(identityserverEndpoint)
    .WithReference(rabbitmq)
    .WithReference(cache)
    .WaitFor(cache)
    .WaitFor(rabbitmq)
    .PublishAsDockerFile();
 

var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
    .WithUrl("https://localhost:5002")
    .WithReference(apiEndpoint)
    .WithReference(rabbitmq) 
    .WaitFor(rabbitmq)
    .PublishAsDockerFile();
 
var angularWebUi =
builder.AddNpmApp("angularWebUi", "../PlainBridge.Web/PlainBridge.Web.UI")
    .WithUrl("http://localhost:12007")
    .WithReference(apiEndpoint)
    .WithReference(identityserverEndpoint)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile(); 
 
builder.AddDockerfile("PlainBridge-AppHost", "relative/context/path")
    .WithReference(apiEndpoint)
    .WithReference(identityserverEndpoint)
    .WithReference(serverEndpoint)
    .WithReference(angularWebUi);   

await builder.Build().RunAsync();
