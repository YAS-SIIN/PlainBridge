


using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlainBridge.Api.Domain.HostAggregate.ValueObjects;
using PlainBridge.Api.Domain.UserAggregate;
using PlainBridge.SharedDomain.Base;
using PlainBridge.SharedDomain.Base.Enums;
using PlainBridge.SharedDomain.Base.ValueObjects;

namespace PlainBridge.Api.Domain.HostAggregate;

public class HostApplication : BaseAggregate<long>
{
    public AppId AppId { get; set; }
    public string Name { get; set; }
    public HostDomain Domain { get; set; }
    public InternalUrl InternalUrl { get; set; }

    [ForeignKey("UserId")]
    public long UserId { get; set; }

    public User User { get; set; }

    public static HostApplication Create(string name, string domain, string internalUrl, long userId, string? description)
    {
        EnsureName(name);

        return new HostApplication
        {
            AppId = AppId.CreateUniqueId(),
            Name = name,
            Domain = HostDomain.Create(domain),
            InternalUrl = InternalUrl.Create(internalUrl),
            UserId = userId,
            State = RowStateEnum.DeActive,
            Description = description
        };

    }

    public void Update(string name, string domain, string internalUrl, string? description)
    {
        EnsureName(name);

        Name = name;
        Domain = HostDomain.Create(domain);
        InternalUrl = InternalUrl.Create(internalUrl);
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

}

public class HostApplicationTypeConfiguration : BaseEntityTypeConfiguration<HostApplication, long>
{
    public override void Configure(EntityTypeBuilder<HostApplication> builder)
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

        builder.OwnsOne(u => u.Domain, owned =>
        {
            owned.Property(p => p.HostDomainName)
                 .HasColumnName(nameof(Domain))
                 .IsRequired().HasMaxLength(200);
             
            owned.WithOwner();
        }); 

        builder.OwnsOne(u => u.InternalUrl, owned =>
        {
            owned.Property(p => p.InternalUrlValue)
                 .HasColumnName(nameof(InternalUrl))
                 .IsRequired().HasMaxLength(200);
             
            owned.WithOwner();
        }); 

        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);  

    }
}