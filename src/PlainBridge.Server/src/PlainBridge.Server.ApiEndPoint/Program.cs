using PlainBridge.Server.ApiEndPoint;
using PlainBridge.Server.Application.DTOs;
try
{

	var builder = WebApplication.CreateBuilder(args);

	builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

	builder.AddServiceDefaults();

	builder.AddRedisClient(connectionName: "cache");
	builder.AddRedisDistributedCache(connectionName: "cache");
	builder.AddRabbitMQClient(connectionName: "messaging");

	builder.Services.AddServerProjectServices();
	 
	var app = builder.Build();

	app.AddUsers();

	await app.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
}


public partial class Program { }