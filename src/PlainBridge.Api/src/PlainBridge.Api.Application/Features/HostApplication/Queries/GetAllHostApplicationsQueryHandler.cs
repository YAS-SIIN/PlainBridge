
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.Shared.Application.DTOs;
using PlainBridge.Shared.Application.Enums;
using PlainBridge.Shared.Application.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Queries;

public class GetAllHostApplicationsQueryHandler(ILogger<GetAllHostApplicationsQueryHandler> _logger, MainDbContext _dbContext) : IRequestHandler<GetAllHostApplicationsQuery, List<HostApplicationDto>>
{
    public async Task<List<HostApplicationDto>> Handle(GetAllHostApplicationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllHostApplicationsQuery.");
        var list = await _dbContext.HostApplications.Include(a => a.User).AsNoTracking().ToListAsync(cancellationToken);
        return list.Select(x => new HostApplicationDto
        {
            Id = x.Id,
            AppId = x.AppId.ViewId,
            UserId = x.UserId,
            UserName = x.User.UserName.UserNameValue,
            Name = x.Name,
            Domain = x.Domain.HostDomainName,
            InternalUrl = x.InternalUrl.InternalUrlValue,
            Description = x.Description,
            State = (RowStateEnum)x.State
        }).ToList();
    }
}
