
using PlainBridge.Client.ApiEndPoint;
using PlainBridge.Client.Application.DTOs;
try
{

	var builder = WebApplication.CreateBuilder(args);

	builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

	builder.AddServiceDefaults();

	builder.AddRabbitMQClient(connectionName: "messaging");

	builder.Services.AddClientProjectServices();

	builder.WebHost.ConfigureKestrel(options =>
	{
		options.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("CLIENT_PROJECT_PORT") ?? "5055"), listenOptions =>
		{
			listenOptions.UseHttps(); // HTTP/3 requires HTTPS
			listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
		});
	});

	var app = builder.Build();

	app.MapDefaultEndpoints();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.MapOpenApi();
	}

	app.AddUsers();

	await app.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
}