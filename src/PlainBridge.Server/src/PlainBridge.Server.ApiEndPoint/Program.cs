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

	builder.WebHost.ConfigureKestrel(options =>
	{
		options.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("SERVER_PROJECT_PORT") ?? "5052"), listenOptions =>
		{
			listenOptions.UseHttps(); // HTTP/3 requires HTTPS
			listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
		});
	});

	var app = builder.Build();

	app.AddUsers();

	await app.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
}


public partial class Program { }