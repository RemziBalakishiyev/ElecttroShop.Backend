using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class ForwardingFreightConfiguration : BaseCommonEntityConfiguration<ForwardingFreight>
{
    public override void Configure(EntityTypeBuilder<ForwardingFreight> builder)
    {
        base.Configure(builder);

        builder.ToTable("ForwardingFreights");

        builder.Property(ff => ff.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ff => ff.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(ff => ff.Email)
            .IsUnique();

        builder.Property(ff => ff.Phone)
            .HasMaxLength(20);

        builder.HasIndex(ff => ff.Phone)
            .IsUnique()
            .HasFilter("[Phone] IS NOT NULL");

        builder.Property(ff => ff.Address)
            .HasMaxLength(500);

        builder.Property(ff => ff.TaxId)
            .HasMaxLength(50);

        builder.Property(ff => ff.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Navigation property
        builder.HasMany(ff => ff.Shippers)
            .WithOne(s => s.ForwardingFreight)
            .HasForeignKey(s => s.ForwardingFreightId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

