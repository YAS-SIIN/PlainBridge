

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Infrastructure.Data.Context;

namespace PlainBridge.Server.Application.Services.HostApplication;

public class HostApplicationService
{
    private readonly ILogger<HostApplicationService> _logger;

    public HostApplicationService(ILogger<HostApplicationService> logger)
    {
        _logger = logger; 
    }



}
