using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class OrderConfiguration : BaseCommonEntityConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        builder.ToTable("Orders");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Subtotal Money Value Object
        builder.OwnsOne(o => o.Subtotal, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Subtotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("SubtotalCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        // VAT Money Value Object
        builder.OwnsOne(o => o.Vat, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Vat")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("VatCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        // Total Money Value Object
        builder.OwnsOne(o => o.Total, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Total")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("TotalCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        // Relationships
        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

