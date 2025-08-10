using System.Globalization; 
using PlainBridge.IdentityServer.EndPoint; 
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.IdentityServer.EndPoint.Endpoints;
using Elastic.Serilog.Sinks;

using Serilog;

var simpleLogger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

simpleLogger.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddElasticsearchClient(connectionName: "elasticsearch");

    builder.Host.UseSerilog((ctx, services, lc) => lc
        .WriteTo.Elasticsearch()
        .Enrich.FromLogContext()
        .ReadFrom.Services(services), preserveStaticLogger: true);


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
    simpleLogger.Fatal(ex, "Unhandled exception");
}
finally
{
    simpleLogger.Information("Shut down complete"); 
}

