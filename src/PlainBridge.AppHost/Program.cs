var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("Cache");

var apiService = builder.AddProject<Projects.PlainBridge_Api_ApiEndPoint>("ApiEndPoint");

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

builder.AddProject<Projects.PlainBridge_Server_ApiEndPoint>("ServerApiEndPoint");

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

builder.Build().Run();
