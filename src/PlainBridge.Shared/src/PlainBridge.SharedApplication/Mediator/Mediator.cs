
using Microsoft.Extensions.DependencyInjection;

namespace PlainBridge.SharedApplication.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // Send - For command/query operations
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetService(handlerType);
 
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for {requestType.Name}");

        // Get pipeline behaviors
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<IRequest<TResponse>, TResponse>>().ToList();

        // Create the request pipeline
        RequestHandlerDelegate<TResponse> pipeline = () =>
        {
            var method = handlerType.GetMethod("Handle");
            if (method == null)
                throw new InvalidOperationException($"No Handle method found for handler type {handlerType.Name}");

            var result = method.Invoke(handler, new object[] { request, cancellationToken });
            if (result is not Task<TResponse> task)
                throw new InvalidOperationException($"Handle method did not return Task<{typeof(TResponse).Name}>");

            return task;
        };

        // Apply behaviors in reverse order (so first registered runs first)
        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var currentPipeline = pipeline;
            pipeline = () => behavior.Handle(request, currentPipeline, cancellationToken);
        }

        return await pipeline();
    }

    // Send - For command/query operations
    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetService(handlerType);
 
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for {requestType.Name}");

        // Get pipeline behaviors
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<IRequest>>().ToList();

        // Create the request pipeline
        RequestHandlerDelegate pipeline = () =>
        {
            var method = handlerType.GetMethod("Handle");
            if (method == null)
                throw new InvalidOperationException($"No Handle method found for handler type {handlerType.Name}");

            var result = method.Invoke(handler, new object[] { request, cancellationToken });
            if (result is not Task task)
                throw new InvalidOperationException($"Handle method did not return Task");


            return task;
        };

        // Apply behaviors in reverse order (so first registered runs first)
        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var currentPipeline = pipeline;
            pipeline = () => behavior.Handle(request, currentPipeline, cancellationToken);
        }

        await pipeline();
    }

}