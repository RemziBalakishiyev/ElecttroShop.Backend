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

        builder.Property(pi => pi.ImageUrl)
            .HasMaxLength(2048);

        builder.Property(pi => pi.PublicId)
            .HasMaxLength(512);

        builder.Property(pi => pi.ImagePath)
            .HasMaxLength(1024);

        builder.Property(pi => pi.FileName)
            .HasMaxLength(512);

        builder.Property(pi => pi.ContentType)
            .HasMaxLength(128);

        builder.Property(pi => pi.Size);

        builder.Property(pi => pi.StorageProvider)
            .HasMaxLength(64)
            .HasDefaultValue("Cloudinary");

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pi => new { pi.ProductId, pi.DisplayOrder });
        builder.HasIndex(pi => new { pi.ProductId, pi.IsPrimary })
            .HasFilter("\"IsPrimary\" = true");
    }
}
