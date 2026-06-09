

using PlainBridge.Shared.Application.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class UpdateStateServerApplicationCommand : IRequest
{
    public long Id { get; init; }
    public bool IsActive { get; init; }
}
