

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.Application.Services.HostApplication;

public class HostApplicationService(ILogger<HostApplicationService> _logger, MainDbContext _dbContext, IEventBus _eventBus) : IHostApplicationService
{
    public async Task<IList<HostApplicationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all host applications.");
        var list = await _dbContext.HostApplications.Include(a=>a.User).AsNoTracking().ToListAsync(cancellationToken);

        return list.Select(x => new HostApplicationDto
        {
            Id = x.Id,
            AppId = x.AppId.ViewId,
            UserId = x.UserId,
            UserName = x.User.Username,
            Name = x.Name,
            Domain = x.Domain.HostDomainName,
            InternalUrl = x.InternalUrl.InternalUrlValue,
            Description = x.Description,
            State = (RowStateEnum)x.State
        }).ToList();
    }

    public async Task<HostApplicationDto?> GetAsync(long id, long userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting host application by Id: {Id}", id);
        var hostApp = await _dbContext.HostApplications.Include(a => a.User).AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (hostApp == null)
        {
            _logger.LogWarning("Host application with Id {Id} not found.", id);
            throw new NotFoundException(id);
        }

        return new HostApplicationDto
        {
            Id = hostApp.Id,
            AppId = hostApp.AppId.ViewId,
            UserId = hostApp.UserId,
            UserName = hostApp.User.Username,
            Name = hostApp.Name,
            Domain = hostApp.Domain.HostDomainName,
            InternalUrl = hostApp.InternalUrl.InternalUrlValue,
            Description = hostApp.Description, 
            State = (RowStateEnum)hostApp.State
        };
    }

    public async Task<Guid> CreateAsync(HostApplicationDto hostApplication, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating host application with domain: {Domain}", hostApplication.Domain);
        var isDomainExists = await _dbContext.HostApplications.AnyAsync(x => x.Domain.HostDomainName == hostApplication.Domain, cancellationToken);
        if (isDomainExists)
        {
            _logger.LogWarning("Host application with domain {Domain} already exists.", hostApplication.Domain);
            throw new DuplicatedException(hostApplication.Domain);
        }
        var app = Domain.HostAggregate.HostApplication.Create(hostApplication.Name, hostApplication.Domain, hostApplication.InternalUrl, hostApplication.UserId, hostApplication.Description);
        
        await _dbContext.HostApplications.AddAsync(app, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Host application with domain {Domain} created. AppId: {AppId}", app.Domain, app.AppId);
        await _eventBus.PublishAsync<string>("Host_Application_Created", cancellationToken);

        return app.AppId.ViewId;
    }

    public async Task UpdateAsync(HostApplicationDto hostApplication, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating host application with Id: {Id}", hostApplication.Id);
        var app = await _dbContext.HostApplications.FindAsync(hostApplication.Id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Host application with Id {Id} not found for update.", hostApplication.Id);
            throw new NotFoundException(hostApplication.Id);
        }

        var isDomainExists = await _dbContext.HostApplications.AnyAsync(x => x.Domain.HostDomainName == hostApplication.Domain && x.Id != hostApplication.Id, cancellationToken);

        if (isDomainExists)
        {
            _logger.LogWarning("Another host application with domain {Domain} already exists.", hostApplication.Domain);
            throw new ApplicationException(hostApplication.Domain);
        }

        app.Update(hostApplication.Name, hostApplication.Domain, hostApplication.InternalUrl, hostApplication.Description);
        

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Host application with Id {Id} updated.", hostApplication.Id);
        await _eventBus.PublishAsync<string>("Host_Application_Updated", cancellationToken);
    }

    public async Task UpdateStateAsync(long id, bool isActive, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating host application state. Id: {Id}, IsActive: {IsActive}", id, isActive);
        var app = await _dbContext.HostApplications.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Host application with Id {Id} not found for state update.", id);
            throw new NotFoundException(id);
        }
         

        if (isActive)
            app.Activate();
        else
            app.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _eventBus.PublishAsync<string>("Host_Application_State_Updated", cancellationToken);
         
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting host application with Id: {Id}", id);
        var app = await _dbContext.HostApplications.FindAsync(id, cancellationToken);
        if (app == null)
        {
            _logger.LogWarning("Host application with Id {Id} not found for deletion.", id);
            throw new NotFoundException(id);
        }

        _dbContext.HostApplications.Remove(app);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Host application with Id {Id} deleted.", id);
        await _eventBus.PublishAsync<string>("Host_Application_Deleted", cancellationToken);
    }
}
