 
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Queries;

public class GetAllHostApplicationsQuery : IRequest<List<HostApplicationDto>>
{
    public long Id { get; init; }
}
