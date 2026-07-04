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

        // Normalized columns for PostgreSQL unique constraints (stored generated)
        builder.Property<string>("NormalizedAttributeType")
            .HasMaxLength(50)
            .HasComputedColumnSql("LOWER(TRIM(\"AttributeType\"))", stored: true);

        builder.HasIndex("CategoryId", "NormalizedAttributeType")
            .IsUnique()
            .HasDatabaseName("UX_CategoryAttributes_CategoryId_NormalizedAttributeType")
            .HasFilter("\"IsDeleted\" = false");

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
            .HasMaxLength(CategoryAttributeValue.MaxValueLength);

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

        // Case-sensitive trimmed value uniqueness (16GB != 16gb — matches application ValueEquals)
        builder.Property<string>("NormalizedValue")
            .HasMaxLength(CategoryAttributeValue.MaxValueLength)
            .HasComputedColumnSql("TRIM(\"Value\")", stored: true);

        builder.HasIndex("CategoryAttributeId", "NormalizedValue")
            .IsUnique()
            .HasDatabaseName("UX_CategoryAttributeValues_CategoryAttributeId_NormalizedValue");

        // Indexes
        builder.HasIndex(cav => new { cav.CategoryAttributeId, cav.DisplayOrder });
    }
}






