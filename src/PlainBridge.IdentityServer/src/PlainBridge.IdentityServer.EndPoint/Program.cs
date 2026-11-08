using PlainBridge.IdentityServer.EndPoint;
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.IdentityServer.EndPoint.Endpoints;
try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.AddServiceDefaults();

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    builder.Services.AddIDSProjectServices(builder.Configuration);
     
    var app = builder.Build();

    app.MapGroup("api/")
        .MapUserEndpoint();

    app.AddUsers();
    await app.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Exception");
}

public partial class Program { }