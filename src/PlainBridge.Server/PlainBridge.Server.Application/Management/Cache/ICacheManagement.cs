using PlainBridge.Server.Application.Management.WebSocketManagement;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Management.Cache;

public interface ICacheManagement
{
    HostApplicationDto SetHostApplication(string host, HostApplicationDto value);
    ServerApplicationDto SetServerApplication(Guid viewId, ServerApplicationDto value);
    ServerApplicationDto SetServerApplication(int port, ServerApplicationDto value);
    IWebSocketManagement SetWebSocket(string host, IWebSocketManagement value);
    bool TryGetHostApplication(string host, out HostApplicationDto value);
    bool TryGetServerApplication(Guid viewId, out ServerApplicationDto value);
    bool TryGetServerApplication(int port, out ServerApplicationDto value);
    bool TryGetWebSocket(string host, out IWebSocketManagement value);
    void RemoveWebSocket(string host);
}