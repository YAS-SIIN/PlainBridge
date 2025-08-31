
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Domain.HostAggregate;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Commands;

public class UpdateHostApplicationCommandHandler(ILogger<UpdateHostApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<UpdateHostApplicationCommand, Guid>
{
    public async Task<Guid> Handle(UpdateHostApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating host application with Id: {Id}", request.Id);
        var app = await _dbContext.HostApplications.FindAsync(request.Id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Host application with Id {Id} not found for update.", request.Id);
            throw new NotFoundException(request.Id);
        }

        var isDomainExists = await _dbContext.HostApplications.AnyAsync(x => x.Domain.HostDomainName == request.Domain && x.Id != request.Id, cancellationToken);

        if (isDomainExists)
        {
            _logger.LogWarning("Another host application with domain {Domain} already exists.", request.Domain);
            throw new ApplicationException(request.Domain);
        }

        app.Update(request.Name, request.Domain, request.InternalUrl, request.Description);

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Host application with Id {Id} updated.", request.Id);
        await _eventBus.PublishAsync<string>("Host_Application_Updated", cancellationToken);

        return app.AppId.ViewId;
    }
}
