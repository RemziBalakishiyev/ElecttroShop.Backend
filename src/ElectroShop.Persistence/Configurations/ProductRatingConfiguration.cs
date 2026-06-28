using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class ProductRatingConfiguration : BaseCommonEntityConfiguration<ProductRating>
{
    public override void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductRatings");

        builder.Property(r => r.RatingValue)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(ProductRating.MaxCommentLength);

        builder.HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => new { r.ProductId, r.UserId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
