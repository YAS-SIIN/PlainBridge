 

namespace PlainBridge.SharedApplication.Mediator;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}