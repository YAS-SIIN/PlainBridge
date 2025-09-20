
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.WebSocket;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.WebSocket;

namespace PlainBridge.Server.ApiEndPoint.Middleware;

public class WebSocketProxyMiddleware(RequestDelegate _next, ILogger<WebSocketProxyMiddleware> _logger, IServiceProvider _serviceProvider)
{
    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation("WebSocketProxyMiddleware invoked for request: {Method} {Path}", context.Request.Method, context.Request.Path);
        CancellationToken cancellationToken = context.RequestAborted;
        using var scope = _serviceProvider.CreateScope();
        var hostApplicationService = scope.ServiceProvider.GetRequiredService<IHostApplicationService>();
        var webSocketService = scope.ServiceProvider.GetRequiredService<IWebSocketService>();
        var applicationSettings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApplicationSettings>>().Value;
        IWebSocketManagement webSocketManagement = null!;
        
        var requestId = Guid.NewGuid().ToString();
        var host = context.Request.Host;

        var project = await hostApplicationService.GetByHostAsync(host!.Value!, cancellationToken);

        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocketConnection = await context.WebSockets.AcceptWebSocketAsync();
            var loggerWebSocketManagement = scope.ServiceProvider.GetRequiredService<ILogger<WebSocketManagement>>();
            webSocketManagement = new WebSocketManagement(loggerWebSocketManagement, webSocketConnection);
            await webSocketService.HandleWebSocketAsync(webSocketManagement, project, cancellationToken);
        }
        else
        {
            await _next(context);
        }
    }
}
