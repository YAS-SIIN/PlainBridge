

using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Commands;

public class UpdateStateHostApplicationCommand : IRequest
{
    public long Id { get; init; }
    public bool IsActive { get; init; }
}
