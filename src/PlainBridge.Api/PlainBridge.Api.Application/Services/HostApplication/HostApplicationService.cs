

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Handler.Bus;
using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.Application.Services.HostApplication;

public class HostApplicationService(ILogger<HostApplicationService> _logger, MainDbContext _dbContext, IBusHandler _busHandler) : IHostApplicationService
{ 


    public async Task<IList<HostApplicationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var list = await _dbContext.HostApplications.AsNoTracking().ToListAsync(cancellationToken);

        return list.Select(x => new HostApplicationDto
        {
            Id = x.Id,
            AppId = x.AppId,
            Name = x.Name,
            Domain = x.Domain,
            InternalUrl = x.InternalUrl
        }).ToList();
    }

    public async Task<HostApplicationDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var hostApp = await _dbContext.HostApplications.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (hostApp == null)
            throw new NotFoundException(id);


        return hostApp == null ? null : new HostApplicationDto
        {
            Id = hostApp.Id,
            AppId = hostApp.AppId,
            Name = hostApp.Name,
            Domain = hostApp.Domain,
            InternalUrl = hostApp.InternalUrl
        };
    }

    public async Task<Guid> CreateAsync(HostApplicationDto hostApplication, CancellationToken cancellationToken)
    { 
        var isDomainExists = await _dbContext.HostApplications.AnyAsync(x => x.Domain == hostApplication.Domain, cancellationToken);
        if (isDomainExists)
            throw new DuplicatedException(hostApplication.Domain);

        var app = new Domain.Entities.HostApplication
        {
            AppId = Guid.NewGuid(),
            Name = hostApplication.Name,
            Domain = hostApplication.Domain,
            InternalUrl = hostApplication.InternalUrl
        };

        await _dbContext.HostApplications.AddAsync(app, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _busHandler.PublishAsync<string>("Host_Application_Created", cancellationToken);

        return app.AppId;
    }

    public async Task UpdateAsync(HostApplicationDto hostApplication, CancellationToken cancellationToken)
    {
        var app = await _dbContext.HostApplications.FindAsync(hostApplication.Id, cancellationToken);
        if (app == null)
            throw new NotFoundException(hostApplication.Id);

        var isDomainExists = await _dbContext.HostApplications.AnyAsync(x => x.Domain == hostApplication.Domain && x.Id != hostApplication.Id, cancellationToken);

        if (isDomainExists)
            throw new ApplicationException(hostApplication.Domain);
        app.Domain = hostApplication.Domain;
        app.InternalUrl = hostApplication.InternalUrl;
        app.Name = hostApplication.Name;
        app.Description = hostApplication.Description;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _busHandler.PublishAsync<string>("Host_Application_Updated", cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var app = await _dbContext.HostApplications.FindAsync(id, cancellationToken);
        if (app == null)
            throw new NotFoundException(id);

        _dbContext.HostApplications.Remove(app);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _busHandler.PublishAsync<string>("Host_Application_Deleted", cancellationToken);
    }

}
