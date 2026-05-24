using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class ProductImageConfiguration : BaseEntityConfiguration<ProductImage>
{
    public override void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductImages");

        builder.Property(pi => pi.ImageId)
            .IsRequired();

        builder.Property(pi => pi.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pi => pi.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pi => new { pi.ProductId, pi.DisplayOrder });
        builder.HasIndex(pi => new { pi.ProductId, pi.IsPrimary })
            .HasFilter("\"IsPrimary\" = true");
    }
}






