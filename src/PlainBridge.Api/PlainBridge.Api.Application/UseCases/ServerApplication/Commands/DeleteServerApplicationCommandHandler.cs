
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class DeleteServerApplicationCommandHandler(ILogger<DeleteServerApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<DeleteServerApplicationCommand>
{
    public async Task Handle(DeleteServerApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting server application with Id: {Id}", request);
        var app = await _dbContext.ServerApplications.FindAsync(request.Id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Server application with Id {Id} not found for deletion.", request.Id);
            throw new NotFoundException(request.Id);
        }

        _dbContext.ServerApplications.Remove(app);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Server application with Id {Id} deleted.", request.Id);
        await _eventBus.PublishAsync<string>("Server_Application_Deleted", cancellationToken);
    }
}
