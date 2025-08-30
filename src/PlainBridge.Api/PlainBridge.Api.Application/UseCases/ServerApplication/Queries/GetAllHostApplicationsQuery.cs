using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Queries;

public class GetAllHostApplicationsQuery : IRequest<List<HostApplicationDto>>
{
}
