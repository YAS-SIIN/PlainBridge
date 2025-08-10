using System.Globalization; 
using PlainBridge.IdentityServer.EndPoint; 
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.IdentityServer.EndPoint.Endpoints; 

using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", formatProvider: CultureInfo.InvariantCulture)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    builder.AddServiceDefaults();

    builder.Services.AddOptions<ApplicationSettings>().Bind(builder.Configuration.GetSection("ApplicationSettings"));
     
    builder.Services.AddIDSProjectServices();

    // if you want to use server-side sessions: https://blog.duendesoftware.com/posts/20220406_session_management/
    // then enable it
    //isBuilder.AddServerSideSessions();
    //
    // and put some authorization on the admin/management pages
    //builder.Services.AddAuthorization(options =>
    //       options.AddPolicy("admin",
    //           policy => policy.RequireClaim("sub", "1"))
    //   );
    //builder.Services.Configure<RazorPagesOptions>(options =>
    //    options.Conventions.AuthorizeFolder("/ServerSideSessions", "admin"));


    // Configure Kestrel to support HTTP/3
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5003, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTP/3 requires HTTPS
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
        });
    });
  
    var app = builder.Build();

    app.MapGroup("api/")
        .MapUserEndpoint();

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

