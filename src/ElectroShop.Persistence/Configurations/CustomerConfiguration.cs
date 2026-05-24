using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class CustomerConfiguration : BaseCommonEntityConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.ToTable("Customers");

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.HasIndex(c => c.Phone)
            .IsUnique()
            .HasFilter("[Phone] IS NOT NULL");

        builder.Property(c => c.PasswordHash)
            .HasMaxLength(500);
    }
}

