
using PlainBridge.Shared.Application.DTOs;
using PlainBridge.Shared.Application.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Queries;

public class GetAllServerApplicationsQuery : IRequest<List<ServerApplicationDto>>
{
    public long Id { get; init; }
}
