using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class SaleExpenseConfiguration : BaseCommonEntityConfiguration<SaleExpense>
{
    public override void Configure(EntityTypeBuilder<SaleExpense> builder)
    {
        base.Configure(builder);

        builder.ToTable("SaleExpenses");

        builder.Property(e => e.ExpenseType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .HasMaxLength(SaleExpense.MaxDescriptionLength);

        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(e => e.Sale)
            .WithMany(s => s.Expenses)
            .HasForeignKey(e => e.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.SaleId);
        builder.HasIndex(e => e.ExpenseType);
    }
}
