using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Handler.Cache;

public interface ICache
{
    HostApplicationDto SetHostApplication(string host, HostApplicationDto value);
    bool TryGetHostApplication(string host, out HostApplicationDto value);

    ServerApplicationDto SetServerApplication(Guid viewId, ServerApplicationDto value);
    bool TryGetServerApplication(Guid viewId, out ServerApplicationDto value);
}