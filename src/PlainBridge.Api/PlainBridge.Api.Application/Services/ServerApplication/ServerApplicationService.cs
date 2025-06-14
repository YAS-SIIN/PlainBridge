

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Handler.Bus;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;

using RabbitMQ.Client;

namespace PlainBridge.Api.Application.Services.ServerApplication;

public class ServerApplicationService : IServerApplicationService
{
    private readonly ILogger<ServerApplicationService> _logger;
    private readonly MainDbContext _dbContext;
    private readonly IBusHandler _busHandler;

    public ServerApplicationService(ILogger<ServerApplicationService> logger, MainDbContext dbContext, IBusHandler busHandler)
    {
        _logger = logger;
        _dbContext = dbContext;
        _busHandler = busHandler;
    }

    public async Task<IList<ServerApplicationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var serverApplication = await _dbContext.ServerApplications.AsNoTracking().ToListAsync(cancellationToken);

        return serverApplication.Select(x => new ServerApplicationDto
        {
            Id = x.Id,
            AppId = x.AppId,
            Name = x.Name,
            InternalPort = x.InternalPort
        }).ToList();
    }

    public async Task<ServerApplicationDto> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var serverApp = await _dbContext.ServerApplications.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (serverApp == null)
            throw new NotFoundException(id);
        return new ServerApplicationDto
        {
            Id = serverApp.Id,
            AppId = serverApp.AppId,
            Name = serverApp.Name,
            InternalPort = serverApp.InternalPort
        };
    }

    public async Task<Guid> CreateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken)
    {
        if (serverApplication.InternalPort < 1 || serverApplication.InternalPort > 65535)
            throw new ApplicationException("Port range is not valid");

        var app = new Domain.Entities.ServerApplication
        {
            AppId = Guid.NewGuid(),
            Name = serverApplication.Name,
            InternalPort = serverApplication.InternalPort
        };

        await _dbContext.ServerApplications.AddAsync(app, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _busHandler.PublishAsync<string>("Server_Application_Created", cancellationToken);

        return app.AppId;
    }

    public async Task UpdateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken)
    {
        var app = await _dbContext.ServerApplications.FindAsync(serverApplication.Id, cancellationToken);
        if (app == null)
            throw new NotFoundException(serverApplication.Id);

        app.InternalPort = serverApplication.InternalPort;
        app.Name = serverApplication.Name;
       
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _busHandler.PublishAsync<string>("Server_Application_Updated", cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var app = await _dbContext.ServerApplications.FindAsync(id, cancellationToken);
        if (app == null)
            throw new NotFoundException(id);

        _dbContext.ServerApplications.Remove(app);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _busHandler.PublishAsync<string>("Server_Application_Deleted", cancellationToken);
    }

}
