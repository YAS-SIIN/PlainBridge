using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.WebSocketManagement;
using PlainBridge.Server.Application.Services.ServerApplication;
using PlainBridge.Server.Application.Services.WebSocket;

namespace PlainBridge.Server.ApiEndPoint.Middlewares;

public class WebSocketProxyMiddleware(RequestDelegate _next, ILogger<WebSocketProxyMiddleware> _logger, IServiceProvider _serviceProvider)
{
    public async Task Invoke(HttpContext context, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var serverApplicationService = scope.ServiceProvider.GetRequiredService<IServerApplicationService>();
        var webSocketService = scope.ServiceProvider.GetRequiredService<IWebSocketService>();
        var applicationSettings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApplicationSettings>>().Value;
        IWebSocketManagement webSocketManagement = null;
        
        var requestId = Guid.NewGuid().ToString();
        var host = context.Request.Host;

        var project = serverApplicationService.GetByHost(host.Value);

        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocketConnection = await context.WebSockets.AcceptWebSocketAsync();
            webSocketManagement = new WebSocketManagement(webSocketConnection);
            await webSocketService.HandleWebSocketAsync(webSocketManagement, project, cancellationToken);
        }
        else
        {
            await _next(context);
        }
    }
}
