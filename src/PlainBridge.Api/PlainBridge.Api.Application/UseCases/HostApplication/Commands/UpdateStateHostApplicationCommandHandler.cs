

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Commands;

public class UpdateStateHostApplicationCommandHandler(ILogger<UpdateStateHostApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<UpdateStateHostApplicationCommand>
{
    public async Task Handle(UpdateStateHostApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating host application state. Id: {Id}, IsActive: {IsActive}", request.Id, request.IsActive);
        var app = await _dbContext.HostApplications.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Host application with Id {Id} not found for state update.", request.Id);
            throw new NotFoundException(request.Id);
        }


        if (request.IsActive)
            app.Activate();
        else
            app.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Host_Application_State_Updated", cancellationToken);

    }
}
