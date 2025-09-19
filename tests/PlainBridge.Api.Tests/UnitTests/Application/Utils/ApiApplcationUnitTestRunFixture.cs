 
using Microsoft.EntityFrameworkCore;
using PlainBridge.Api.Domain.HostAggregate;
using PlainBridge.Api.Domain.ServerAggregate;
using PlainBridge.Api.Domain.ServerAggregate.Enums; 
using PlainBridge.Api.Domain.UserAggregate;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context; 


namespace PlainBridge.Api.Tests.UnitTests.Application.Utils;


public class ApiApplcationUnitTestRunFixture : IAsyncLifetime
{


    public MainDbContext MemoryMainDbContext;
    public ApiApplcationUnitTestRunFixture()
    {

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<MainDbContext>();

        dbContextOptionsBuilder.UseInMemoryDatabase($"MainDbContext");
        DbContextOptions<MainDbContext>? contextOptions = dbContextOptionsBuilder.Options;
        MemoryMainDbContext = new MainDbContext(contextOptions);

    }

    public async Task InitializeAsync()
    {
        await SeedDataAsync();

    }

    /// <summary>
    /// Initializing new data
    /// </summary>
    public async Task SeedDataAsync()
    {

        List<User> userList = new List<User>();
        int countUser = 3;
        var userExistences = await MemoryMainDbContext.Users.AnyAsync();
        if (!userExistences)
        {
            // Add new customer

            for (int i = 1; i <= countUser; i++)
            { 
                userList.Add(User.Create(
                    i.ToString(),
                    $"TestUser{i}",
                    $"TestUser{i}@PlainBridge.Com",
                    $"+98912111222{i}",
                    $"TestName{i}",
                    $"TestFamily{i}", ""));
            }
            MemoryMainDbContext.Users.AddRange(userList);

            await MemoryMainDbContext.SaveChangesAsync(CancellationToken.None);

        }

        var hostApplicationExistences = await MemoryMainDbContext.HostApplications.AnyAsync();
        if (!hostApplicationExistences)
        {
            // Add new project  
            for (int i = 1; i <= userList.Count; i++)
            {
                var hostApp = HostApplication.Create(
                    $"TestTitle{i}",
                    $"TestDomain{i}",
                    $"http://localhost:300{i}",
                    MemoryMainDbContext.Users.ToList()[i - 1].Id,
                    $"This is test application {i}"
                );
                await  MemoryMainDbContext.HostApplications.AddAsync(hostApp);
            }
        }

        var serverApplicationExistences = await MemoryMainDbContext.ServerApplications.AnyAsync();
        if (!serverApplicationExistences)
        {
            // Add new project  
            for (int i = 1; i <= userList.Count; i++)
            {
                var serverApp = ServerApplication.Create(
                    Guid.Empty,
                    ServerApplicationTypeEnum.SharePort,
                    $"TestTitle{i}",
                    2020 + i,
                    MemoryMainDbContext.Users.ToList()[i - 1].Id,
                    $"This is test application {i}"
                );
                await MemoryMainDbContext.ServerApplications.AddAsync(serverApp);
            }
        }

        await MemoryMainDbContext.SaveChangesAsync(CancellationToken.None);

    }


    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

}

