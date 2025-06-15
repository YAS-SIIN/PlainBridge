using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;
using System.Text;
using System;
using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Api.Application.Services.HostApplication;
using RabbitMQ.Client;

namespace PlainBridge.Server.ApiEndPoint.Middlewares;

public class HttpRequestProxyMiddleware
{
    private readonly ILogger<HttpRequestProxyMiddleware> _logger;
    private readonly ResponseCompletionSourcesManagement _responseCompletionSourcesManagement;
    private readonly IHostApplicationService _hostApplicationService;
    private readonly ApplicationSetting _applicationSetting;
    private readonly IConnection _connection;
    private Dictionary<string, bool> _initializedQueues = new Dictionary<string, bool>();

    private readonly RequestDelegate _next;

    public HttpRequestProxyMiddleware(ILogger<HttpRequestProxyMiddleware> logger, RequestDelegate next, ResponseCompletionSourcesManagement responseCompletionSourcesManagement, IHostApplicationService hostApplicationService, ApplicationSetting applicationSetting, IConnection connection)
    {
        _next = next;
        _responseCompletionSourcesManagement = responseCompletionSourcesManagement;
        _hostApplicationService = hostApplicationService;
        _applicationSetting = applicationSetting;
        _logger = logger; 
    }
     
}
