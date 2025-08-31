
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Domain.HostAggregate;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class UpdateServerApplicationCommandHandler(ILogger<UpdateServerApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<UpdateServerApplicationCommand, Guid>
{
    public async Task<Guid> Handle(UpdateServerApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating server application with Id: {Id}", request.Id);
        var app = await _dbContext.ServerApplications.FindAsync(request.Id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Server application with Id {Id} not found for update.", request.Id);
            throw new NotFoundException(request.Id);
        }

        var isDomainExists = await _dbContext.ServerApplications.AnyAsync(x => x.Domain.HostDomainName == request.Domain && x.Id != request.Id, cancellationToken);

        if (isDomainExists)
        {
            _logger.LogWarning("Another server application with domain {Domain} already exists.", request.Domain);
            throw new ApplicationException(request.Domain);
        }

        app.Update(request.Name, request.Domain, request.InternalUrl, request.Description);

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Server application with Id {Id} updated.", request.Id);
        await _eventBus.PublishAsync<string>("Server_Application_Updated", cancellationToken);

        return app.AppId.ViewId;
    }
}
