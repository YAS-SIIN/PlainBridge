
using PlainBridge.Client.ApiEndPoint;
using PlainBridge.Client.Application.DTOs;
try
{

	var builder = WebApplication.CreateBuilder(args);

	builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

	builder.AddServiceDefaults();

	builder.AddRabbitMQClient(connectionName: "messaging");

	builder.Services.AddClientProjectServices(builder.Configuration);
	 
	var app = builder.Build();
	 

	app.AddUsers();

	await app.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
	throw;
}