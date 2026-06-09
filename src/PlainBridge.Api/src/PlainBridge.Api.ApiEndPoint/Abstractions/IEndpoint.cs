 
namespace PlainBridge.Api.ApiEndPoint.Abstractions;
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}