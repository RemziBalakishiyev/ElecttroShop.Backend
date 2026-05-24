using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class CategoryAttributeConfiguration : BaseCommonEntityConfiguration<CategoryAttribute>
{
    public override void Configure(EntityTypeBuilder<CategoryAttribute> builder)
    {
        base.Configure(builder);

        builder.ToTable("CategoryAttributes");

        builder.Property(ca => ca.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ca => ca.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ca => ca.AttributeType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ca => ca.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ca => ca.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(ca => ca.Category)
            .WithMany()
            .HasForeignKey(ca => ca.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ca => ca.Values)
            .WithOne(cav => cav.CategoryAttribute)
            .HasForeignKey(cav => cav.CategoryAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ca => new { ca.CategoryId, ca.DisplayOrder });
        builder.HasIndex(ca => ca.AttributeType);
    }
}

public class CategoryAttributeValueConfiguration : BaseEntityConfiguration<CategoryAttributeValue>
{
    public override void Configure(EntityTypeBuilder<CategoryAttributeValue> builder)
    {
        base.Configure(builder);

        builder.ToTable("CategoryAttributeValues");

        builder.Property(cav => cav.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cav => cav.DisplayValue)
            .HasMaxLength(200);

        builder.Property(cav => cav.ColorCode)
            .HasMaxLength(7); // Hex color code: #RRGGBB

        builder.Property(cav => cav.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(cav => cav.CategoryAttribute)
            .WithMany(ca => ca.Values)
            .HasForeignKey(cav => cav.CategoryAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(cav => new { cav.CategoryAttributeId, cav.DisplayOrder });
    }
}






