


using IdentityModel.Client;

using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.DTOs;
using Microsoft.Extensions.Options;

using System.Text;
using System.Text.Json;

namespace PlainBridge.Api.Application.Services.Customer;

public class CustomerService(ILogger<CustomerService> _logger, IHttpClientFactory _httpClientFactory, IOptions<ApplicationSetting> _applicationSetting)
{

     
}
