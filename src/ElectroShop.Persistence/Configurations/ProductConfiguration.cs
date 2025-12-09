using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class ProductConfiguration : BaseCommonEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.ToTable("Products");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        // SKU Value Object configuration
        builder.OwnsOne(p => p.Sku, sku =>
        {
            sku.Property(s => s.Value)
                .HasColumnName("Sku")
                .IsRequired()
                .HasMaxLength(50);

            sku.HasIndex(s => s.Value)
                .IsUnique();
        });

        // Money Value Object configuration
        builder.OwnsOne(p => p.Price, price =>
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

        builder.Property(p => p.VatRate)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.IsBanner)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.DisplayOrder)
            .IsRequired(false);

        // Indexes
        // PostgreSQL syntax for filtered indexes
        builder.HasIndex(p => p.IsBanner)
            .HasFilter("\"IsBanner\" = true");

        builder.HasIndex(p => new { p.IsFeatured, p.DisplayOrder })
            .HasFilter("\"IsFeatured\" = true");

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationships
        builder.HasMany(p => p.ProductImages)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductVariants)
            .WithOne(pv => pv.Product)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
