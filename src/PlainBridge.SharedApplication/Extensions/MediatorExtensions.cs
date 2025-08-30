

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.SharedApplication.Extensions;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly)
    {
        // Register mediator
        services.AddScoped<IMediator, Mediator.Mediator>(); // Use the fully qualified name if Mediator is a class inside the Mediator namespace

        // Register handlers
        RegisterHandlers(services, assembly);

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        // Register request handlers
        var requestHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        foreach (var handlerType in requestHandlerTypes)
        {
            var handlerInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            services.AddTransient(handlerInterface, handlerType);
        }

    }
}
