

using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class DeleteServerApplicationCommand : IRequest
{
    public long Id { get; set; }
}
