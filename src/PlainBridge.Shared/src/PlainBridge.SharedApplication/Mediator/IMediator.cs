 

namespace PlainBridge.SharedApplication.Mediator;


public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default); 
    Task Send(IRequest request, CancellationToken cancellationToken = default); 
}