using System.Globalization;
using System.Text;

using Duende.IdentityServer;
using Duende.IdentityServer.Licensing;
using Duende.IdentityServer.Test;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PlainBridge.IdentityServer.EndPoint;
using PlainBridge.IdentityServer.EndPoint.Domain.Entities;
using PlainBridge.IdentityServer.EndPoint.Endpoints;
using PlainBridge.IdentityServer.EndPoint.ErrorHandling;
using PlainBridge.IdentityServer.EndPoint.Infrastructure.Data;

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
      
    builder.Services.AddRazorPages();

    var isBuilder = builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    })
        .AddTestUsers(TestUsers.Users)
        .AddLicenseSummary();

    // in-memory, code config
    isBuilder.AddInMemoryIdentityResources(Config.IdentityResources);
    isBuilder.AddInMemoryApiScopes(Config.ApiScopes);
    isBuilder.AddInMemoryClients(Config.Clients);


    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("IdentityDb"));

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


    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            // register your IdentityServer with Google at https://console.developers.google.com
            // enable the Google+ API
            // set the redirect URI to https://localhost:5001/signin-google
            options.ClientId = "copy client ID from Google here";
            options.ClientSecret = "copy client secret from Google here";
        });

    // Configure Kestrel to support HTTP/3
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5003, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTP/3 requires HTTPS
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
        });
    });
    builder.Services.AddExceptionHandler<ErrorHandler>();

    builder.Services.AddLocalApiAuthentication();

    var app = builder.Build();

    app.MapGroup("api/")
        .MapUserEndpoint();


    app.UseSerilogRequestLogging();
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            var usage = app.Services.GetRequiredService<LicenseUsageSummary>();
            Console.Write(Summary(usage));
            Console.ReadKey();
        });
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseIdentityServer();
    app.UseAuthorization();

    app.MapRazorPages()
        .RequireAuthorization();

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

static string Summary(LicenseUsageSummary usage)
{
    var sb = new StringBuilder();
    sb.AppendLine("IdentityServer Usage Summary:");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  License: {usage.LicenseEdition}");
    var features = usage.FeaturesUsed.Count > 0 ? string.Join(", ", usage.FeaturesUsed) : "None";
    sb.AppendLine(CultureInfo.InvariantCulture, $"  Business and Enterprise Edition Features Used: {features}");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.ClientsUsed.Count} Client Id(s) Used");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.IssuersUsed.Count} Issuer(s) Used");

    return sb.ToString();
}
