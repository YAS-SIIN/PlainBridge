var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.PlainBridge_Api_ApiService>("apiservice");

//builder.AddProject<Projects.PlainBridge_Api_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.Build().Run();
