using System.Globalization;
using System.Text;
using Duende.IdentityServer;
using Duende.IdentityServer.Licensing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.IdentityServer.EndPoint.ErrorHandling;
using PlainBridge.IdentityServer.EndPoint.Infrastructure.Data;
using Serilog;

namespace PlainBridge.IdentityServer.EndPoint;

public static class DependencyResolver
{
    public static IServiceCollection AddIDSProjectServices(this IServiceCollection services)
    {

        var appSettings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSettings>>();

        services.AddRazorPages();
        services.AddIDSProjectDatabase();
        services.AddProblemDetails();

        var isBuilder = services.AddIdentityServer(options =>
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
        isBuilder.AddInMemoryClients(Config.Clients(appSettings.Value));

        // Use developer signing credentials in Development to avoid file system key store writes


        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                // register your IdentityServer with Google at https://console.developers.google.com
                // enable the Google+ API
                // set the redirect URI to https://localhost:5001/signin-google
                options.ClientId = "copy client ID from Google here";
                options.ClientSecret = "copy client secret from Google here";
            });

        services.AddExceptionHandler<ErrorHandler>();

        services.AddLocalApiAuthentication();

        services.AddHttpServices();

        return services;
    }

    public static IServiceCollection AddIDSProjectDatabase(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("PlainBridgeIDSDBContext"));

        return services;
    }



    public static WebApplication AddUsers(this WebApplication app)
    {

        app.UseExceptionHandler();
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

        return app;
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

}
