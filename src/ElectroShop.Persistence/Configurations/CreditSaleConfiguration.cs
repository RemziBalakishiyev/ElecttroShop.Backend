using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class CreditSaleConfiguration : BaseCommonEntityConfiguration<CreditSale>
{
    public override void Configure(EntityTypeBuilder<CreditSale> builder)
    {
        base.Configure(builder);

        builder.ToTable("CreditSales");

        builder.Property(c => c.CustomerName)
            .HasMaxLength(200);

        builder.Property(c => c.CustomerPhone)
            .HasMaxLength(50);

        builder.Property(c => c.ProductSource)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.ProductName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(c => c.ProductCode)
            .HasMaxLength(50);

        builder.Property(c => c.CategoryName)
            .HasMaxLength(200);

        builder.Property(c => c.CostPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.SalePrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.TotalCostAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.TotalSaleAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.TotalExpenses)
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(c => c.GrossProfit)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.NetProfit)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.Quantity)
            .IsRequired();

        builder.Property(c => c.CreditDate)
            .IsRequired();

        builder.Property(c => c.DueDate)
            .IsRequired();

        builder.Property(c => c.DebtDurationDays)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Note)
            .HasMaxLength(1000);

        builder.Property(c => c.ConvertedAt)
            .IsRequired(false);

        builder.HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.ConvertedSale)
            .WithMany()
            .HasForeignKey(c => c.ConvertedSaleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.CreditDate);
        builder.HasIndex(c => c.DueDate);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ProductId);
        builder.HasIndex(c => c.CustomerPhone);
        builder.HasIndex(c => c.ConvertedSaleId);
        builder.HasIndex(c => new { c.IsDeleted, c.CreditDate });
    }
}
