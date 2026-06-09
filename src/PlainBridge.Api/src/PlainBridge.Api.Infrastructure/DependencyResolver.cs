

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlainBridge.Api.Infrastructure.ExternalServices.Identity;
using PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
using PlainBridge.Api.Infrastructure.Persistence.Cache;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;

namespace PlainBridge.Api.Infrastructure;


public static class DependencyResolver
{
    public static IServiceCollection AddApiInfrastructureProjectServices(this IServiceCollection services)
    {

        // Add services to the container 


        services.AddScoped<IEventBus, RabbitMQEventBus>();
        services.AddScoped<ICacheManagement, CacheManagement>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddDbContext<MainDbContext>(options => options.UseInMemoryDatabase("PlainBridgeApiDBContext"));


        return services;
    }

}

