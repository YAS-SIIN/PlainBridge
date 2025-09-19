
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Server.Application.Services.ApiExternalBus;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.ServerApplication;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;

namespace PlainBridge.Server.Tests.UnitTests.Services;

[Collection("ServerUnitTestRun")]
public class ApiExternalBusServiceTests
{
    private readonly IApiExternalBusService _apiExternalBusService;

    private readonly Mock<ILogger<ApiExternalBusService>> _mockLogger = new();
    private readonly Mock<IHostApplicationService> _mockHostApplicationService = new();
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly Mock<IServerApplicationService> _mockServerApplicationService = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public ApiExternalBusServiceTests()
    {
            _apiExternalBusService = new ApiExternalBusService(_mockLogger.Object, _mockConnection.Object, _mockHostApplicationService.Object, _mockServerApplicationService.Object);
    }

    [Fact]
    public async Task InitializeAsync_ShouldSetupQueueAndInitializeConsumer()
    {
        // Arrange  
        var projects = new List<HostApplicationDto>() { 
            new HostApplicationDto { 
                State = RowStateEnum.Active, 
                Domain = "PlainBridge.local" 
            } };
        var _mockChannel = new Mock<IChannel>();
 
        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);
  
        // Act
        await _apiExternalBusService.InitializeAsync(CancellationToken.None);

        // Assert
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);
        _mockHostApplicationService.Verify(m => m.UpdateHostApplicationAsync(_cancellationToken), Times.Once);
        _mockServerApplicationService.Verify(m => m.UpdateServerApplicationAsync(_cancellationToken), Times.Once);
        
    }
}
