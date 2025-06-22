
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PlainBridge.Api.Domain.Common;

namespace PlainBridge.Api.Domain.Entities;

public class Customer : BaseEntity<long>
{  
    public string ExternalId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
}

public class CustomerTypeConfiguration : BaseEntityTypeConfiguration<Customer, long>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(150); 
        builder.Property(b => b.Family).IsRequired().HasMaxLength(150);
        builder.Property(b => b.Email).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Username).IsRequired().HasMaxLength(150);


    }
}