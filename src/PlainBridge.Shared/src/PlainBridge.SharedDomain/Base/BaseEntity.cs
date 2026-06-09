



using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlainBridge.SharedDomain.Base.Enums;

namespace PlainBridge.SharedDomain.Base;

/// <summary>
/// Base Entity Class Which is base of the every entity. 
/// </summary>
/// <typeparam name="TId"></typeparam>
public abstract class BaseEntity<TId>
{
    /// <summary>
    /// Identity Key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TId? Id { get; set; }

    /// <summary>
    /// Create DateTime
    /// </summary>
    [Required]
    public DateTime CreateDateTime { get; set; }

    /// <summary>
    /// Update DateTime
    /// </summary>
    [Required]
    public DateTime UpdateDateTime { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [StringLength(250)]
    public string? Description { get; set; }

    /// <summary>
    /// State of the Row
    /// </summary>
    public RowStateEnum State { get; set; }
}


public class BaseEntityTypeConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TKey>
    where TKey : notnull
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedOnAdd();
          
        builder.Property(b => b.Description)
            .HasMaxLength(250);

        builder.Property(x => x.State)
            .HasConversion<string>()
            .IsRequired();
    }
}
