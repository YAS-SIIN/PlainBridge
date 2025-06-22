

using Castle.Core.Resource;

using Microsoft.EntityFrameworkCore;

using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Infrastructure.Data.Context;


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
                userList.Add(new User
                {
                    AppId = Guid.NewGuid(),
                    Username = $"TestUser{i}",
                    Email = $"TestUser{i}@PlainBridge.Com",
                    PhoneNumber = $"0912111222{i}",
                    Name = $"TestName{i}",
                    Family = $"User{i}",
                    ExternalId = i.ToString()
                });
            }
            MemoryMainDbContext.Users.AddRange(userList);

        }

        var hostApplicationExistences = await MemoryMainDbContext.HostApplications.AnyAsync();
        if (!hostApplicationExistences)
        {
            // Add new project  
            for (int i = 1; i <= userList.Count; i++)
            {
               await  MemoryMainDbContext.HostApplications.AddAsync(
                   new HostApplication
                   {
                       AppId = Guid.NewGuid(),
                       Name = $"TestTitle{i}",
                       Domain = $"TestDomain{i}",
                       User = userList[i - 1],
                       InternalUrl = $"http://localhost:300{i}",
                       CreateDateTime = DateTime.Now,
                       UpdateDateTime = DateTime.Now,
                       State = Domain.Enums.RowStateEnum.Active,
                   }
               );
            }
        }

        var serverApplicationExistences = await MemoryMainDbContext.ServerApplications.AnyAsync();
        if (!serverApplicationExistences)
        {
            // Add new project  
            for (int i = 1; i <= userList.Count; i++)
            {
                await MemoryMainDbContext.ServerApplications.AddAsync(
                   new ServerApplication
                   {
                       AppId = Guid.NewGuid(),
                       Name = $"TestTitle{i}",
                       InternalPort = 2020 + i,
                       User = userList[i - 1],
                       CreateDateTime = DateTime.Now,
                       UpdateDateTime = DateTime.Now,
                       State = Domain.Enums.RowStateEnum.Active,
                   }
               );
            }
        }

        await MemoryMainDbContext.SaveChangesAsync(CancellationToken.None);

    }


    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

}

