

using Castle.Core.Resource;

using Microsoft.EntityFrameworkCore;

using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Domain.HostAggregate;
using PlainBridge.Api.Domain.ServerAggregate;
using PlainBridge.Api.Domain.ServerAggregate.ValueObjects;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.SharedDomain.Base.Enums;
using PlainBridge.SharedDomain.Base.ValueObjects;


namespace PlainBridge.Api.UnitTests;


public class TestRunFixture : IAsyncLifetime
{


    public MainDbContext MemoryMainDbContext;
    public TestRunFixture()
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
                    AppId.CreateUniqueId()!.ToString()!,
                    $"TestUser{i}",
                    $"TestUser{i}@PlainBridge.Com",
                    $"+98912111222{i}",
                    $"TestName{i}",
                    $"TestFamily{i}", ""));
            }
            MemoryMainDbContext.Users.AddRange(userList);

        }

        var hostApplicationExistences = await MemoryMainDbContext.HostApplications.AnyAsync();
        if (!hostApplicationExistences)
        {
            // Add new project  
            for (int i = 1; i <= userList.Count; i++)
            {
                var hostApp = HostApplication.Create(
                    $"TestTitle{i}",
                    $"TestDomain{i}.com",
                    $"http://localhost:300{i}",
                    userList[i - 1].Id,
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
                    Domain.ServerAggregate.Enums.ServerApplicationTypeEnum.SharePort,
                    $"TestTitle{i}",
                    2020 + i,
                    userList[i - 1].Id,
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

