

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Commands;

public class CreateHostApplicationCommandHandler(ILogger<CreateHostApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<CreateHostApplicationCommand, Guid>
{
    public async Task<Guid> Handle(CreateHostApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating host application with domain: {Domain}", request.Domain);
        var isDomainExists = await _dbContext.HostApplications.AnyAsync(x => x.Domain.HostDomainName == request.Domain, cancellationToken);
        if (isDomainExists)
        {
            _logger.LogWarning("Host application with domain {Domain} already exists.", request.Domain);
            throw new DuplicatedException(request.Domain);
        }
        var app = Domain.HostAggregate.HostApplication.Create(request.Name, request.Domain, request.InternalUrl, request.UserId, request.Description);

        await _dbContext.HostApplications.AddAsync(app, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Host application with domain {Domain} created. AppId: {AppId}", app.Domain, app.AppId);
        await _eventBus.PublishAsync<string>("Host_Application_Created", cancellationToken);

        return app.AppId.ViewId;
    }
}
