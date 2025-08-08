

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;

using RabbitMQ.Client;

namespace PlainBridge.Api.Application.Services.ServerApplication;

public class ServerApplicationService(ILogger<ServerApplicationService> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IServerApplicationService
{
    public async Task<IList<ServerApplicationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all server applications.");
        var serverApplication = await _dbContext.ServerApplications.Include(a => a.User).AsNoTracking().ToListAsync(cancellationToken);

        return serverApplication.Select(x => new ServerApplicationDto
        {
            Id = x.Id,
            AppId = x.AppId,
            UserId = x.UserId,
            UserName = x.User.Username,
            Name = x.Name,
            InternalPort = x.InternalPort,
            Description = x.Description,
            State = (RowStateEnum)x.State,
        }).ToList();
    }
     
    public async Task<ServerApplicationDto> GetAsync(long id, long userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting server application by Id: {Id}", id);
        var serverApp = await _dbContext.ServerApplications.Include(a => a.User).AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        if (serverApp == null)
        {
            _logger.LogWarning("Server application with Id: {Id} not found.", id);
            throw new NotFoundException(id);
        }
        return new ServerApplicationDto
        {
            Id = serverApp.Id,
            AppId = serverApp.AppId,
            UserId = serverApp.UserId,
            UserName = serverApp.User.Username,
            Name = serverApp.Name,
            InternalPort = serverApp.InternalPort,
            Description = serverApp.Description,
            State = (RowStateEnum)serverApp.State
        };
    }

    public async Task<Guid> CreateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating server application with Name: {Name}", serverApplication.Name);
        if (serverApplication.InternalPort < 1 || serverApplication.InternalPort > 65535)
        {
            _logger.LogError("Port range is not valid: {Port}", serverApplication.InternalPort);
            throw new ApplicationException("Port range is not valid");
        }

        if (serverApplication.ServerApplicationType == ServerApplicationTypeEnum.UsePort && (!serverApplication.ServerApplicationAppId.HasValue || serverApplication.ServerApplicationAppId == Guid.Empty))
        {
            _logger.LogError("ServerApplicationViewId is required for UsePort type.");
            throw new ArgumentNullException(nameof(ServerApplicationDto.ServerApplicationAppId));
        }

        if (serverApplication.ServerApplicationType == ServerApplicationTypeEnum.UsePort && !_dbContext.ServerApplications.Any(x => x.AppId == serverApplication.ServerApplicationAppId))
        {
            _logger.LogError("Referenced ServerApplicationViewId not found: {ViewId}", serverApplication.ServerApplicationAppId);
            throw new NotFoundException(nameof(serverApplication), new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(nameof(ServerApplicationDto.ServerApplicationAppId), serverApplication.ServerApplicationAppId.Value) });
        }

        var app = new Domain.Entities.ServerApplication
        {
            AppId = Guid.NewGuid(),
            ServerApplicationViewId = serverApplication.ServerApplicationAppId,
            Name = serverApplication.Name,
            UserId = serverApplication.UserId,
            InternalPort = serverApplication.InternalPort,
            State = (Domain.Enums.RowStateEnum)RowStateEnum.Inactive,
            Description = serverApplication.Description
        };

        await _dbContext.ServerApplications.AddAsync(app, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Server_Application_Created", cancellationToken);

        _logger.LogInformation("Server application created with AppId: {AppId}", app.AppId);
        return app.AppId;
    }

    public async Task UpdateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating server application with Id: {Id}", serverApplication.Id);
        var app = await _dbContext.ServerApplications.FindAsync(serverApplication.Id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Server application with Id: {Id} not found for update.", serverApplication.Id);
            throw new NotFoundException(nameof(ServerApplication), new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(nameof(ServerApplicationDto.Id), serverApplication.Id) });
        }

        app.InternalPort = serverApplication.InternalPort;
        app.Name = serverApplication.Name;
        app.Description = serverApplication.Description;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Server_Application_Updated", cancellationToken);
        _logger.LogInformation("Server application with Id: {Id} updated.", serverApplication.Id);
    }

    public async Task UpdateStateAsync(long id, bool isActive, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating server application state. Id: {Id}, IsActive: {IsActive}", id, isActive);
        var app = await _dbContext.ServerApplications.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Server application with Id: {Id} not found for state update.", id);
            throw new NotFoundException(id);
        }
        app.State = isActive ? Domain.Enums.RowStateEnum.Active : Domain.Enums.RowStateEnum.Inactive;
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Server_Application_State_Updated", cancellationToken);
         
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting server application with Id: {Id}", id);
        var app = await _dbContext.ServerApplications.FindAsync(id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Server application with Id: {Id} not found for deletion.", id);
            throw new NotFoundException(id);
        }

        _dbContext.ServerApplications.Remove(app);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Server_Application_Deleted", cancellationToken);
        _logger.LogInformation("Server application with Id: {Id} deleted.", id);
    }
}
