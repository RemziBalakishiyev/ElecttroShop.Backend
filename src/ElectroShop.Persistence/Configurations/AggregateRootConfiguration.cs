using ElectroShop.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

/// <summary>
/// Aggregate Root base configuration
/// RowVersion - Optimistic Concurrency Control üçün
/// </summary>
public abstract class AggregateRootConfiguration<TAggregateRoot> : BaseCommonEntityConfiguration<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    public override void Configure(EntityTypeBuilder<TAggregateRoot> builder)
    {
        base.Configure(builder);

        // PostgreSQL xmin system column — avtomatik yenilənən concurrency token
        builder.Property(a => a.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion();
    }
}

