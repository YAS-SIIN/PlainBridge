

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class CreateServerApplicationCommandHandler(ILogger<CreateServerApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<CreateServerApplicationCommand, Guid>
{
    public async Task<Guid> Handle(CreateServerApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating server application with domain: {Domain}", request.Domain);
        var isDomainExists = await _dbContext.ServerApplications.AnyAsync(x => x.Domain.ServerDomainName == request.Domain, cancellationToken);
        if (isDomainExists)
        {
            _logger.LogWarning("Server application with domain {Domain} already exists.", request.Domain);
            throw new DuplicatedException(request.Domain);
        }
        var app = Domain.ServerAggregate.ServerApplication.Create(request.Name, request.Domain, request.InternalUrl, request.UserId, request.Description);

        await _dbContext.ServerApplications.AddAsync(app, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Server application with domain {Domain} created. AppId: {AppId}", app.Domain, app.AppId);
        await _eventBus.PublishAsync<string>("Server_Application_Created", cancellationToken);

        return app.AppId.ViewId;
    }
}
