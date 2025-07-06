using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlainBridge.IdentityServer.EndPoint.Domain.Entities;

namespace PlainBridge.IdentityServer.EndPoint.Infrastructure.Data;


public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<IdentityUser> IdentityUsers { get; set; } = null!;
    public DbSet<IdentityRole> IdentityRoles { get; set; } = null!;

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    var pathToExe = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
    //    optionsBuilder.UseSqlite($"Data Source={System.IO.Path.Combine(pathToExe, "database.db")}");
    //}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}