using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PlainBridge.Api.Domain.Common;
using PlainBridge.Api.Domain.Enums;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainBridge.Api.Domain.Entities;

public class ServerApplication : BaseEntity<long>
{ 
    public Guid AppId { get; set; } 
    public Guid? ServerApplicationViewId { get; set; } 
    public string Name { get; set; } 
    public int InternalPort { get; set; } 
    public ServerApplicationTypeEnum ServerApplicationType { get; set; } 
}


public class ServerApplicationTypeConfiguration : BaseEntityTypeConfiguration<ServerApplication, long>
{
    public void Configure(EntityTypeBuilder<ServerApplication> builder)
    {
        base.Configure(builder);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
    }
}