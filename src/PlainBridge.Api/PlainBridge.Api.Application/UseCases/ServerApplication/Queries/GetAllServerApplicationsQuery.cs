
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Queries;

public class GetAllServerApplicationsQuery : IRequest<List<ServerApplicationDto>>
{
    public long Id { get; init; }
}
