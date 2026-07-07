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

        // PostgreSQL sistem xmin sütununa map (Npgsql IsRowVersion).
        // Fiziki "xmin" sütunu yaratmayın — sistem sütunu ilə konflikt yaradır.
        builder.Property(a => a.RowVersion)
            .IsRowVersion();
    }
}

