
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Queries;

public class GetHostApplicationQueryHandler(ILogger<GetHostApplicationQueryHandler> _logger, MainDbContext _dbContext) : IRequestHandler<GetHostApplicationQuery, HostApplicationDto>
{
    public async Task<HostApplicationDto> Handle(GetHostApplicationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting host application by Id: {Id}", request.Id);
        var hostApp = await _dbContext.HostApplications.Include(a => a.User).AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId, cancellationToken);

        if (hostApp == null)
        {
            _logger.LogWarning("Host application with Id {Id} not found.", request.Id);
            throw new NotFoundException(request.Id);
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
}
