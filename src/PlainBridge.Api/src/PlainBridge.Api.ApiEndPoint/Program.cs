
using PlainBridge.Api.ApiEndPoint;
using PlainBridge.Api.Infrastructure.DTOs;
try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.AddServiceDefaults();

    builder.AddRedisClient(connectionName: "cache");
    builder.AddRedisDistributedCache(connectionName: "cache");
    builder.AddRabbitMQClient(connectionName: "messaging");

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    builder.Services.AddApiProjectServices(builder.Configuration);
     
    var app = builder.Build();

    app.AddUsers();

    await app.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
}