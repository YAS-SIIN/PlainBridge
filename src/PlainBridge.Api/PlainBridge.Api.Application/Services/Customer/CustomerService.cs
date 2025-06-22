

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.Identity.Customer;
using PlainBridge.Api.Infrastructure.Data.Context;

namespace PlainBridge.Api.Application.Services.Customer;

public class CustomerService(
    ILogger<CustomerService> _logger,
    IIdentityService _identityService,
    MainDbContext _dbContext) 
{

}
