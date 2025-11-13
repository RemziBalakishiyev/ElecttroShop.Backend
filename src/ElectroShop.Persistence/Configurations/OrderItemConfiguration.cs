using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class OrderItemConfiguration : BaseEntityConfiguration<OrderItem>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("OrderItems");

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        // UnitPrice Money Value Object
        builder.OwnsOne(oi => oi.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("UnitPriceCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        // LineTotal Money Value Object
        builder.OwnsOne(oi => oi.LineTotal, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("LineTotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("LineTotalCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Property(oi => oi.VatRate)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        // Relationships
        builder.HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

