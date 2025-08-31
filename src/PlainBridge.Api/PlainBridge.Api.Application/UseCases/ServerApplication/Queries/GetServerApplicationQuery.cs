

using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Queries;

public class GetServerApplicationQuery : IRequest<ServerApplicationDto>
{
   public long Id { get; init; }
   public long UserId { get; init; }
}
