using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.WebSocketManagement;
using PlainBridge.Server.Application.Services.ServerApplication;
using PlainBridge.Server.Application.Services.WebSocket;

namespace PlainBridge.Server.ApiEndPoint.Middlewares;

public class WebSocketProxyMiddleware(RequestDelegate _next, ILogger<WebSocketProxyMiddleware> _logger, IServerApplicationService _serverApplicationService, IWebSocketService _webSocketService, ApplicationSetting _applicationSetting, IWebSocketManagement _webSocketManagement)
{
    public async Task Invoke(HttpContext context, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        var host = context.Request.Host;

        var project = _serverApplicationService.GetByHost(host.Value);

        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocketConnection = await context.WebSockets.AcceptWebSocketAsync();
            await _webSocketService.HandleWebSocketAsync(_webSocketManagement, project, cancellationToken);
        }
        else
        {
            await _next(context);
        }
    }
}
