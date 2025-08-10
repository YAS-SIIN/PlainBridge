using System.Globalization;
using Microsoft.Extensions.Options;
using PlainBridge.Server.ApiEndPoint;
using PlainBridge.Server.ApiEndPoint.Middlewares;
using PlainBridge.Server.Application.DTOs; 
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));

    builder.AddServiceDefaults();

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", formatProvider: CultureInfo.InvariantCulture)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    // Add services to the container.


    builder.AddRabbitMQClient(connectionName: "messaging");
    // Configure Kestrel to support HTTP/3
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5002, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTP/3 requires HTTPS
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
        });
    });

    var app = builder.Build();

    //app.Services.GetRequiredService<IWebSocketManagement>().();
    app.AddUsers();
    await app.RunAsync();

}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
