
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlainBridge.Api.Domain.HostAggregate;
using PlainBridge.Api.Domain.ServerAggregate;
using PlainBridge.SharedDomain.Base;
using PlainBridge.SharedDomain.Base.Enums;
using PlainBridge.SharedDomain.Base.ValueObjects;

namespace PlainBridge.Api.Domain.Entities;

public class User : BaseEntity<long>
{
    public User()
    {
        ServerApplications = new HashSet<ServerApplication>();
        HostApplications = new HashSet<HostApplication>();
    }
    public AppId AppId { get; set; }
    public string ExternalId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }

    public virtual ICollection<ServerApplication> ServerApplications { get; set; }
    public virtual ICollection<HostApplication> HostApplications { get; set; }


    public static User Create(string externalId, string username, string email, string? phoneNumber, string name, string family, string? description)
    {
        EnsureName(name);
        EnsureExternalId(externalId);
        EnsureUsername(username);
        EnsureEmail(email);
        EnsurePhoneNumber(phoneNumber);
        EnsureFamily(family);


        return new User
        {
            AppId = AppId.CreateUniqueId(),
            Username = username,
            Email = email,
            PhoneNumber = phoneNumber,
            Name = name,
            Family = family,
            ExternalId = externalId
        };

    }

    public void Update(string name, string family, string? description)
    {
        EnsureName(name);
        EnsureFamily(family);

        Name = name;
        Family = family;
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

    private static void EnsureFamily(string family)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(family);
        if (family.Length > 150)
            throw new ApplicationException("Family must be 150 characters or fewer.");
    }

    private static void EnsureUsername(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        if (username.Length > 150)
            throw new ApplicationException("Username must be 150 characters or fewer.");
    }

    private static void EnsureExternalId(string externalId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalId);
    }

    private static void EnsureEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (email.Length > 200)
            throw new ApplicationException("Email must be 200 characters or fewer.");

        //check email format
        if (!new EmailAddressAttribute().IsValid(email))
            throw new ApplicationException("Email format is not valid.");

    }

    private static void EnsurePhoneNumber(string? phoneNumber)
    {
        if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length > 20)
            throw new ApplicationException("Phone number must be 20 characters or fewer.");

        // Optionally, you can add a regex check for phone number format here
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\+?[1-9]\d{1,14}$");
            if (!phoneRegex.IsMatch(phoneNumber))
                throw new ApplicationException("Phone number format is not valid.");
        }
    }

}

public class UserTypeConfiguration : BaseEntityTypeConfiguration<User, long>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
        builder.OwnsOne(u => u.AppId, owned =>
        {
            owned.Property(p => p.ViewId)
                 .HasColumnName(nameof(AppId))
                 .IsRequired();

            owned.HasIndex(u => u.ViewId)
            .IsUnique();

            // No separate table
            owned.WithOwner();
        });

        // Index/unique constraint lives on the owner 

        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
        builder.Property(b => b.Family).IsRequired().HasMaxLength(150);
        builder.Property(b => b.Email).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Username).IsRequired().HasMaxLength(150);
        builder.Property(b => b.PhoneNumber).HasMaxLength(20);


    }
}