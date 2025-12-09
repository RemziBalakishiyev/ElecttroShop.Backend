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

        // SKU Value Object configuration
        builder.OwnsOne(pv => pv.Sku, sku =>
        {
            sku.Property(s => s.Value)
                .HasColumnName("Sku")
                .IsRequired()
                .HasMaxLength(50);

            sku.HasIndex(s => s.Value)
                .IsUnique();
        });

        // Money Value Object configuration
        builder.OwnsOne(pv => pv.Price, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            price.Property(m => m.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Property(pv => pv.Stock)
            .IsRequired();

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


