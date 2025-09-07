using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
 
public interface IEventBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class;
}