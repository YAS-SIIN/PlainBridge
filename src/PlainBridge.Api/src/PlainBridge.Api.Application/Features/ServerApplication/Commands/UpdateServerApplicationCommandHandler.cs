
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Domain.HostAggregate;
using PlainBridge.Api.Domain.ServerAggregate;
using PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class UpdateServerApplicationCommandHandler(ILogger<UpdateServerApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<UpdateServerApplicationCommand, Guid>
{
    public async Task<Guid> Handle(UpdateServerApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating server application with Id: {Id}", request.Id);
        var app = await _dbContext.ServerApplications.FindAsync(request.Id, cancellationToken);
        if (app is null)
        {
            _logger.LogWarning("Server application with Id: {Id} not found for update.", request.Id);
            throw new NotFoundException(nameof(ServerApplication), new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(nameof(ServerApplicationDto.Id), request.Id) });
        }

        app.Update(request.Name, request.InternalPort, request.Description);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Server_Application_Updated", cancellationToken);
        _logger.LogInformation("Server application with Id: {Id} updated.", request.Id);


        return app.AppId.ViewId;
    }
}
