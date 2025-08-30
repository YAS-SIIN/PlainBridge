using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Domain.ServerAggregate.Enums;
using PlainBridge.Api.Domain.ServerAggregate.ValueObjects;
using PlainBridge.SharedDomain.Base;
using PlainBridge.SharedDomain.Base.Enums;
using PlainBridge.SharedDomain.Base.ValueObjects;

namespace PlainBridge.Api.Domain.ServerAggregate;

public class ServerApplication : BaseAggregate<long>
{ 
    public AppId AppId { get; set; } 
    public Guid? ServerApplicationViewId { get; set; } 
    public string Name { get; set; } 
    public InternalPort InternalPort { get; set; } 
    public ServerApplicationTypeEnum ServerApplicationType { get; set; }

    [ForeignKey("UserId")]
    public long UserId { get; set; }

    public User User { get; set; }



    public static ServerApplication Create(Guid? serverApplicationViewId, ServerApplicationTypeEnum serverApplicationType, string name,  int internalPort, long userId, string? description)
    {
        EnsureName(name); 
        EnsureServerApplicationViewId(serverApplicationType, serverApplicationViewId);

        return new ServerApplication
        {
            AppId = AppId.CreateUniqueId(),
            ServerApplicationViewId = serverApplicationViewId,
            ServerApplicationType = serverApplicationType,
            Name = name,
            UserId = userId,
            InternalPort = InternalPort.Create(internalPort),
            State = RowStateEnum.DeActive,
            Description = description
        };

    }

    public void Update(string name, int internalPort, string? description)
    {
        EnsureName(name); 

        Name = name;
        InternalPort = InternalPort.Create(internalPort);
        Description = description; 
    }

    public void Activate()
    {
        State = RowStateEnum.Active;
    }

    public void Deactivate()
    {
        State = RowStateEnum.DeActive;
    }


    private static void EnsureName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > 150)
            throw new ApplicationException("Name must be 150 characters or fewer.");
    }

    private static void EnsureServerApplicationViewId(ServerApplicationTypeEnum serverApplicationType, Guid? serverApplicationViewId)
    {
        if (serverApplicationType == ServerApplicationTypeEnum.UsePort && (serverApplicationViewId is null || serverApplicationViewId == Guid.Empty))
            throw new ApplicationException("ServerApplicationViewId is required when ServerApplicationType is UseAppId.");

    }
}


public class ServerApplicationTypeConfiguration : BaseEntityTypeConfiguration<ServerApplication, long>
{
    public override void Configure(EntityTypeBuilder<ServerApplication> builder)
    {
        base.Configure(builder);

        builder.OwnsOne(u => u.AppId, owned =>
        {
            owned.Property(p => p.ViewId)
                 .HasColumnName(nameof(AppId))
                 .IsRequired();

            owned.HasIndex(u => u.ViewId)
            .IsUnique();
             
            owned.WithOwner();
        });


        builder.OwnsOne(sa => sa.InternalPort, owned =>
        {
            owned.Property(p => p.Port)
                 .HasColumnName(nameof(InternalPort))
                 .IsRequired();
        });
         
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
    }
}