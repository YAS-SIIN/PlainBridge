var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var rabbitmq = builder.AddRabbitMQ("messaging");

var identityserverEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint")
    .WithUrl("https://localhost:5003");

var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
    .WithUrl("https://localhost:5001")
    .WithReference(identityserverEndpoint)
    .WithReference(rabbitmq)
    .WithReference(cache);
//builder.AddProject<Projects.PlainBridge_Api_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

//builder.AddProject<Projects.PlainBridge_Server>("plainbridge-server");

//builder.AddProject<Projects.PlainBridge_Api_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

var serverEndpoint = builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("server-endpoint")
    .WithUrl("https://localhost:5002")
    .WithReference(apiEndpoint)
    .WithReference(rabbitmq);

//builder.AddProject<Projects.PlainBridge_Api_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

//builder.AddProject<Projects.PlainBridge_Api_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

var angularWebUi =
builder.AddNpmApp("angularWebUi", "../PlainBridge.Web/PlainBridge.Web.UI")
    .WithUrl("http://localhost:12007")
    .WithReference(apiEndpoint)
    .WithReference(identityserverEndpoint)
    .WithExternalHttpEndpoints();

//builder.AddProject<Projects.PlainBridge_Api_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

//builder.AddProject<Projects.PlainBridge_Server>("plainbridge-server");

//builder.AddProject<Projects.PlainBridge_Api_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

await builder.Build().RunAsync();
