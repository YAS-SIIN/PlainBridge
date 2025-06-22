
using Microsoft.EntityFrameworkCore;

using PlainBridge.Api.Domain.Entities;

namespace PlainBridge.Api.Infrastructure.Data.Context;


public class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
    }

    public DbSet<ServerApplication> ServerApplications { get; set; }
    public DbSet<HostApplication> HostApplications { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        new ServerApplicationTypeConfiguration().Configure(modelBuilder.Entity<ServerApplication>());
        new HostApplicationTypeConfiguration().Configure(modelBuilder.Entity<HostApplication>());
        new CustomerTypeConfiguration().Configure(modelBuilder.Entity<Customer>());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly); base.OnModelCreating(modelBuilder);
    }


    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("CreateDateTime") != null))
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreateDateTime").CurrentValue = DateTime.Now;
                continue;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property("CreateDateTime").IsModified = false;
                entry.Property("UpdateDateTime").CurrentValue = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("CreateDateTime") != null))
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreateDateTime").CurrentValue = DateTime.Now;
                continue;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property("CreateDateTime").IsModified = false;
                entry.Property("UpdateDateTime").CurrentValue = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }


}