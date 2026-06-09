 
using PlainBridge.Shared.Application.DTOs;
using PlainBridge.Shared.Application.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Queries;

public class GetAllHostApplicationsQuery : IRequest<List<HostApplicationDto>>
{
    public long Id { get; init; }
}
