
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PlainBridge.Api.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlainBridge.Api.Domain.Enums;

namespace PlainBridge.Api.Domain.Common;

/// <summary>
/// Base Entity Class Which is base of the every entity. 
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class BaseEntity<TKey>
{
    /// <summary>
    /// Identity Key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TKey? Id { get; set; }

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
    }
}
