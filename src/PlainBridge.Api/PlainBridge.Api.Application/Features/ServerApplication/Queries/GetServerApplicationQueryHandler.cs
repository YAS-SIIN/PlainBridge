
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Queries;

public class GetServerApplicationQueryHandler(ILogger<GetServerApplicationQueryHandler> _logger, MainDbContext _dbContext) : IRequestHandler<GetServerApplicationQuery, ServerApplicationDto>
{
    public async Task<ServerApplicationDto> Handle(GetServerApplicationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting server application by Id: {Id}", request.Id);
        var serverApp = await _dbContext.ServerApplications.Include(a => a.User).AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId, cancellationToken);
        if (serverApp == null)
        {
            _logger.LogWarning("Server application with Id: {Id} not found.", request.Id);
            throw new NotFoundException(request.Id);
        }
        return new ServerApplicationDto
        {
            Id = serverApp.Id,
            AppId = serverApp.AppId.ViewId,
            ServerApplicationAppId = serverApp.ServerApplicationViewId != Guid.Empty ? serverApp.ServerApplicationViewId.ToString() : null,
            UserId = serverApp.UserId,
            UserName = serverApp.User.UserName.UserNameValue,
            Name = serverApp.Name,
            InternalPort = serverApp.InternalPort.Port,
            Description = serverApp.Description,
            State = (RowStateEnum)serverApp.State
        };
    }
}
