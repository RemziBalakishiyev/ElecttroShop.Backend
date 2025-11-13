using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class BrandConfiguration : BaseCommonEntityConfiguration<Brand>
{
    public override void Configure(EntityTypeBuilder<Brand> builder)
    {
        base.Configure(builder);

        builder.ToTable("Brands");

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(b => b.Name)
            .IsUnique();
    }
}

