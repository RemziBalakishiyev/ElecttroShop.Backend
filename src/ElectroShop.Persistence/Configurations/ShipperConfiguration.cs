using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class ShipperConfiguration : BaseCommonEntityConfiguration<Shipper>
{
    public override void Configure(EntityTypeBuilder<Shipper> builder)
    {
        base.Configure(builder);

        builder.ToTable("Shippers");

        builder.Property(s => s.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(s => s.Email)
            .IsUnique();

        builder.Property(s => s.Phone)
            .HasMaxLength(20);

        builder.HasIndex(s => s.Phone)
            .IsUnique()
            .HasFilter("[Phone] IS NOT NULL");

        builder.Property(s => s.Address)
            .HasMaxLength(500);

        builder.Property(s => s.PasswordHash)
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // ForwardingFreight ilə əlaqə
        builder.Property(s => s.ForwardingFreightId)
            .IsRequired(false);

        builder.HasIndex(s => s.ForwardingFreightId);

        builder.HasOne(s => s.ForwardingFreight)
            .WithMany(ff => ff.Shippers)
            .HasForeignKey(s => s.ForwardingFreightId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

