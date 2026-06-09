namespace PlainBridge.SharedApplication.Mediator;

public interface IRequest<out TResponse> { }
public interface IRequest { }
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    Task Handle(TRequest request, CancellationToken cancellationToken);
}

