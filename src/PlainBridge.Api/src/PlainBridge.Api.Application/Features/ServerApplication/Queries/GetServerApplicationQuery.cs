

using PlainBridge.Shared.Application.DTOs;
using PlainBridge.Shared.Application.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Queries;

public class GetServerApplicationQuery : IRequest<ServerApplicationDto>
{
   public long Id { get; init; }
   public long UserId { get; init; }
}
