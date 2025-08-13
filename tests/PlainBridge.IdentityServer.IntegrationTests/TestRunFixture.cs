

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.IdentityServer.IntegrationTests;


public class TestRunFixture : IAsyncLifetime
{
    public WebApplicationFactory<Program> WebApplicationFactory { get; }
    public HttpClient InjectedHttpClient { get; }
    public UserDto CreatedUer { get; set; } = new UserDto();
    public TestRunFixture()
    {
        WebApplicationFactory = new WebApplicationFactory<Program>();
        InjectedHttpClient = WebApplicationFactory.CreateDefaultClient();
    }

    public async Task InitializeAsync()
    {
        try
        {
            CreatedUer = new UserDto
            {
                Username = "TestUser",
                Email = "testuser@plainbridge.com",
                Name = "Test",
                Family = "User",
                Password = "TestPassword123",
                PhoneNumber = "09121112222"
            };
            var payload = JsonSerializer.Serialize(CreatedUer);

            HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await InjectedHttpClient.PostAsync("/api/user", httpContent, CancellationToken.None);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync();
            Assert.NotNull(res);
            var responseData = JsonSerializer.Deserialize<ResultDto<string>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            CreatedUer.UserId = responseData?.Data ?? Guid.Empty.ToString();

        }
        catch { }
        await Task.CompletedTask;
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

}
