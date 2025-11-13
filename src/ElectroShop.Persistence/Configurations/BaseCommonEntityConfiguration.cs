using ElectroShop.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public abstract class BaseCommonEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : BaseCommonEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(200);

        builder.Property(e => e.UpdatedAtUtc)
            .IsRequired(false);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(200);

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(e => e.IsDeleted);

        // Soft delete için global query filter
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}



