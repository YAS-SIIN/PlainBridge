

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PlainBridge.Api.Domain.Common;
using PlainBridge.Api.Domain.Enums;

using System.ComponentModel.DataAnnotations.Schema;

namespace PlainBridge.Api.Domain.Entities;

public class HostApplication : BaseEntity<long>
{
    public Guid AppId { get; set; } 
    public string Name { get; set; }
    public string Domain { get; set; }
    public string InternalUrl { get; set; } 
    public RowStateEnum State { get; set; }

 
}

public class HostApplicationTypeConfiguration : BaseEntityTypeConfiguration<HostApplication, long>
{
    public void Configure(EntityTypeBuilder<HostApplication> builder)
    {
        base.Configure(builder);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
        builder.Property(b => b.Domain).IsRequired().HasMaxLength(200);
        builder.Property(b => b.InternalUrl).IsRequired().HasMaxLength(200);
    }
}