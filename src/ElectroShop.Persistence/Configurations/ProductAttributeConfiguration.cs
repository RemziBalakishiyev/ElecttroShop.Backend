using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class ProductAttributeConfiguration : BaseCommonEntityConfiguration<ProductAttribute>
{
    public override void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductAttributes");

        builder.Property(pa => pa.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pa => pa.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pa => pa.AttributeType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pa => pa.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pa => pa.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(pa => pa.Product)
            .WithMany(p => p.ProductAttributes)
            .HasForeignKey(pa => pa.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pa => pa.Values)
            .WithOne(pav => pav.ProductAttribute)
            .HasForeignKey(pav => pav.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pa => new { pa.ProductId, pa.DisplayOrder });
    }
}

public class ProductAttributeValueConfiguration : BaseEntityConfiguration<ProductAttributeValue>
{
    public override void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductAttributeValues");

        builder.Property(pav => pav.Value)
            .IsRequired()
            .HasMaxLength(ProductAttributeValue.MaxValueLength);

        builder.Property(pav => pav.DisplayValue)
            .HasMaxLength(500);

        builder.Property(pav => pav.ColorCode)
            .HasMaxLength(7);

        builder.Property(pav => pav.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(pav => pav.ProductAttribute)
            .WithMany(pa => pa.Values)
            .HasForeignKey(pav => pav.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pav => new { pav.ProductAttributeId, pav.DisplayOrder });
    }
}
