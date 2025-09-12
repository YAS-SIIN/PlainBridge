

using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Commands;

public class DeleteHostApplicationCommand : IRequest
{
    public long Id { get; set; }
}
