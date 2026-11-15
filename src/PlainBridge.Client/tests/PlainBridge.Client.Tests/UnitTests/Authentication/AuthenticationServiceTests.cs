
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using PlainBridge.Client.Application.DTOs;
using PlainBridge.Client.Application.Services.Authentication;
using PlainBridge.Client.Application.Services.ClientBus;
using PlainBridge.Shared.Application.DTOs;

namespace PlainBridge.Client.Tests.UnitTests.Authentication;

[Collection("ClientUnitTestRun")]
public class AuthenticationServiceTests
{ 
    //private readonly Mock<AuthenticationService> _mockAuthenticationService = new(); 
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public AuthenticationServiceTests()
    { 
    }


    [Fact]
    public async Task LoginAsync_WhenEveryThinIsOk_ShouldLogin()
    {
        //_mockAuthenticationService.Setup(x => x.LoginAsync(_cancellationToken)).ReturnsAsync(new UserProfileViewDto());
        //var userProfile = await _mockAuthenticationService.Object.LoginAsync(_cancellationToken);
        //Assert.NotNull(userProfile);
         

    }

}
