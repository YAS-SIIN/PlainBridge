using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Handler.Cache;

public interface ICache
{
    HostApplicationDto SetHostApplication(string host, HostApplicationDto value);
    ServerApplicationDto SetServerApplication(Guid viewId, ServerApplicationDto value);
    ServerApplicationDto SetServerApplication(string username, int port, ServerApplicationDto value);
    bool TryGetHostApplication(string host, out HostApplicationDto value);
    bool TryGetServerApplication(Guid viewId, out ServerApplicationDto value);
    bool TryGetServerApplication(string username, int port, out ServerApplicationDto value);
}