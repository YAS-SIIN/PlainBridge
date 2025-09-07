

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Identity;
using PlainBridge.Api.Infrastructure.Messaging;

namespace PlainBridge.Api.Infrastructure;


public static class DependencyResolver
{
    public static IServiceCollection AddApiInfrastructureProjectServices(this IServiceCollection services)
    {

        // Add services to the container 


        services.AddScoped<IEventBus, RabbitMqEventBus>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddDbContext<MainDbContext>(options => options.UseInMemoryDatabase("PlainBridgeApiDBContext"));


        return services;
    }

}

