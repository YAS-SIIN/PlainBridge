var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var rabbitmq = builder.AddRabbitMQ("messaging");

var identityserverEndpoint = builder.AddProject<Projects.PlainBridge_IdentityServer_EndPoint>("identityserver-endpoint");

var apiEndpoint = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("api-endpoint")
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
