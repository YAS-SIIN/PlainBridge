

using PlainBridge.Shared.Application.DTOs;
using PlainBridge.Shared.Application.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Queries;

public class GetHostApplicationQuery : IRequest<HostApplicationDto>
{
   public long Id { get; init; }
   public long UserId { get; init; }
}
