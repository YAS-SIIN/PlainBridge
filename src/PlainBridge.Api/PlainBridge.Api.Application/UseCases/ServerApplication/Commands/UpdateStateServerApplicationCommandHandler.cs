

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class UpdateStateServerApplicationCommandHandler(ILogger<UpdateStateServerApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<UpdateStateServerApplicationCommand>
{
    public async Task Handle(UpdateStateServerApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating server application state. Id: {Id}, IsActive: {IsActive}", request.Id, request.IsActive);
        var app = await _dbContext.ServerApplications.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Server application with Id {Id} not found for state update.", request.Id);
            throw new NotFoundException(request.Id);
        }


        if (request.IsActive)
            app.Activate();
        else
            app.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Server_Application_State_Updated", cancellationToken);

    }
}
