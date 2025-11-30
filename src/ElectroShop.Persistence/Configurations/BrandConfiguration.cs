using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class BrandConfiguration : BaseCommonEntityConfiguration<Brand>
{
    public override void Configure(EntityTypeBuilder<Brand> builder)
    {
        base.Configure(builder);

        builder.ToTable("Brands");

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(b => b.Name)
            .IsUnique();

        builder.Property(b => b.IsPromotional)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(b => b.DisplayOrder)
            .IsRequired(false);

        builder.HasIndex(b => new { b.IsPromotional, b.DisplayOrder })
            .HasFilter("\"IsPromotional\" = true");
    }
}

