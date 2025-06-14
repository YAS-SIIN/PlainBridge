using PlainBridge.Server.Application.DTOs;

using System.Collections.Concurrent;

namespace PlainBridge.Server.Application.Management.ResponseCompletionSources;

public class ResponseCompletionSourcesManagement
{
    private ConcurrentDictionary<string, TaskCompletionSource<HttpResponseDto>> _responseCompletionSources;

    public ConcurrentDictionary<string, TaskCompletionSource<HttpResponseDto>> Sources { get { return _responseCompletionSources; } private set { _responseCompletionSources = value; } }

    public ResponseCompletionSourcesManagement()
    {
        _responseCompletionSources = new ConcurrentDictionary<string, TaskCompletionSource<HttpResponseDto>>();
    }
}
