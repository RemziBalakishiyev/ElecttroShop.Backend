using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class DiscountConfiguration : BaseCommonEntityConfiguration<Discount>
{
    public override void Configure(EntityTypeBuilder<Discount> builder)
    {
        base.Configure(builder);

        builder.ToTable("Discounts");

        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.Percent)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(d => d.StartDate)
            .IsRequired();

        builder.Property(d => d.EndDate)
            .IsRequired(false);

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(d => d.Product)
            .WithMany()
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Brand)
            .WithMany()
            .HasForeignKey(d => d.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Category)
            .WithMany()
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(d => new { d.Type, d.ProductId })
            .HasFilter("\"ProductId\" IS NOT NULL");

        builder.HasIndex(d => new { d.Type, d.BrandId })
            .HasFilter("\"BrandId\" IS NOT NULL");

        builder.HasIndex(d => new { d.Type, d.CategoryId })
            .HasFilter("\"CategoryId\" IS NOT NULL");

        builder.HasIndex(d => new { d.IsActive, d.StartDate, d.EndDate });
    }
}

