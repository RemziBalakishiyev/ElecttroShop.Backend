using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class ProductVariantConfiguration : BaseCommonEntityConfiguration<ProductVariant>
{
    public override void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductVariants");

        builder.Property(pv => pv.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pv => pv.ImageId)
            .IsRequired(false);

        builder.Property(pv => pv.AttributesJson)
            .IsRequired()
            .HasColumnType("jsonb"); // PostgreSQL JSONB type

        // Relationships
        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.ProductVariants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pv => new { pv.ProductId, pv.IsActive });
    }
}



