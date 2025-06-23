
using Microsoft.Extensions.Logging;

using Moq; 
using PlainBridge.Api.Application.Services.Identity;
using PlainBridge.Api.Application.Services.User;

namespace PlainBridge.Api.UnitTests.Application.Services;

[Collection("TestRun")]
public class UserServiceTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly IUserService _userService;
    private readonly Mock<ILogger<UserService>> _mockLoggerUserService;
    private readonly Mock<IIdentityService> _mockIdentityService;

    public UserServiceTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockLoggerUserService = new Mock<ILogger<UserService>>(); 
        _userService = new UserService(_mockLoggerUserService.Object, _fixture.MemoryMainDbContext, _mockIdentityService.Object);
    }

}
