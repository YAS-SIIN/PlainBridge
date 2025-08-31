

using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Queries;

public class GetHostApplicationQuery : IRequest<HostApplicationDto>
{
   public long Id { get; init; }
   public long UserId { get; init; }
}
