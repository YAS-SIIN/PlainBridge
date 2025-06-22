
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PlainBridge.Api.Domain.Common;

namespace PlainBridge.Api.Domain.Entities;

public class User : BaseEntity<long>
{
    public Guid AppId { get; set; }
    public string ExternalId { get; set; }
    public string Username { get; set; } 
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
}

public class UserTypeConfiguration : BaseEntityTypeConfiguration<User, long>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(150); 
        builder.Property(b => b.Family).IsRequired().HasMaxLength(150);
        builder.Property(b => b.Email).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Username).IsRequired().HasMaxLength(150);
        builder.Property(b => b.PhoneNumber).IsRequired().HasMaxLength(20);


    }
}