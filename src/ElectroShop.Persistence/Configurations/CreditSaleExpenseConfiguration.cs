using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class CreditSaleExpenseConfiguration : BaseCommonEntityConfiguration<CreditSaleExpense>
{
    public override void Configure(EntityTypeBuilder<CreditSaleExpense> builder)
    {
        base.Configure(builder);

        builder.ToTable("CreditSaleExpenses");

        builder.Property(e => e.ExpenseType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .HasMaxLength(SaleExpense.MaxDescriptionLength);

        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(e => e.CreditSale)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CreditSaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.CreditSaleId);
        builder.HasIndex(e => e.ExpenseType);
    }
}
