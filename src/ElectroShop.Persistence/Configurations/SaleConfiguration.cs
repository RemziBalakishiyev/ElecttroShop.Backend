using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class SaleConfiguration : BaseCommonEntityConfiguration<Sale>
{
    public override void Configure(EntityTypeBuilder<Sale> builder)
    {
        base.Configure(builder);

        builder.ToTable("Sales");

        builder.Property(s => s.ProductName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(s => s.ProductCode)
            .HasMaxLength(50);

        builder.Property(s => s.CategoryName)
            .HasMaxLength(200);

        builder.Property(s => s.CostPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.SalePrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.TotalCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.TotalSaleAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.TotalExpenses)
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(s => s.Profit)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.Quantity)
            .IsRequired();

        builder.Property(s => s.SaleSource)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.SoldAt)
            .IsRequired();

        builder.Property(s => s.Note)
            .HasMaxLength(1000);

        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.ProductId);
        builder.HasIndex(s => s.CategoryId);
        builder.HasIndex(s => s.SaleSource);
        builder.HasIndex(s => s.SoldAt);
        builder.HasIndex(s => new { s.IsDeleted, s.SoldAt });
    }
}
