

using Microsoft.EntityFrameworkCore;

using PlainBridge.Api.Infrastructure.Data.Context;

using System;
using System.Threading.Tasks;

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


        if (!MemoryMainDbContext.HostApplications.Any())
        {
            // Add new project  
            for (int i = 1; i <= 3; i++)
            {
                MemoryMainDbContext.HostApplications.Add(
                   new Domain.Entities.HostApplication
                   {
                       AppId = Guid.NewGuid(),
                       Name = $"TestTitle{i}",
                       Domain = $"TestDomain{i}",
                       InternalUrl = $"http://localhost:300{i}",
                       CreateDateTime = DateTime.Now,
                       UpdateDateTime = DateTime.Now,
                       State = Domain.Enums.RowStateEnum.Active,
                   }
               );
            }
        }

        if (!MemoryMainDbContext.ServerApplications.Any())
        {
            // Add new project  
            for (int i = 1; i <= 3; i++)
            {
                MemoryMainDbContext.ServerApplications.Add(
                   new Domain.Entities.ServerApplication
                   {
                       AppId = Guid.NewGuid(),
                       Name = $"TestTitle{i}",
                       InternalPort = 2020 + i,
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

