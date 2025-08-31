
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Commands;

public class DeleteHostApplicationCommandHandler(ILogger<DeleteHostApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<DeleteHostApplicationCommand>
{
    public async Task Handle(DeleteHostApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting host application with Id: {Id}", request);
        var app = await _dbContext.HostApplications.FindAsync(request.Id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Host application with Id {Id} not found for deletion.", request.Id);
            throw new NotFoundException(request.Id);
        }

        _dbContext.HostApplications.Remove(app);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Host application with Id {Id} deleted.", request.Id);
        await _eventBus.PublishAsync<string>("Host_Application_Deleted", cancellationToken);
    }
}
