

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Domain.ServerAggregate;
using PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;
using PlainBridge.SharedDomain.Base.ValueObjects;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

public class CreateServerApplicationCommandHandler(ILogger<CreateServerApplicationCommandHandler> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IRequestHandler<CreateServerApplicationCommand, Guid>
{
    public async Task<Guid> Handle(CreateServerApplicationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating server application with Name: {Name}", request.Name);
         
        Guid parsedId = Guid.Empty;
        if (request.ServerApplicationType == ServerApplicationTypeEnum.UsePort)
        {
            Guid.TryParse(request.ServerApplicationAppId, out parsedId);

            if (!_dbContext.ServerApplications.Any(x => x.AppId.ViewId == parsedId))
            {
                _logger.LogError("Referenced ServerApplicationAppId not found: {AppId}", request.ServerApplicationAppId);
                throw new NotFoundException(nameof(request), new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(nameof(ServerApplicationDto.ServerApplicationAppId), parsedId) });
            }
        }
        var app = Domain.ServerAggregate.ServerApplication.Create(parsedId, (Domain.ServerAggregate.Enums.ServerApplicationTypeEnum)request.ServerApplicationType, request.Name, request.InternalPort, request.UserId, request.Description);


        await _dbContext.ServerApplications.AddAsync(app, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Server_Application_Created", cancellationToken);

        _logger.LogInformation("Server application created with AppId: {AppId}", app.AppId);
        return app.AppId.ViewId;
    }
}
