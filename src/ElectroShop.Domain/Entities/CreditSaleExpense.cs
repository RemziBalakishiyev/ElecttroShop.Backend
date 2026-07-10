using ElectroShop.Domain.Enums;
using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class CreditSaleExpense : BaseCommonEntity
{
    public Guid CreditSaleId { get; private set; }
    public CreditSale CreditSale { get; private set; } = default!;
    public ExpenseType ExpenseType { get; private set; }
    public string? Description { get; private set; }
    public decimal Amount { get; private set; }

    private CreditSaleExpense() { }

    private CreditSaleExpense(ExpenseType expenseType, decimal amount, string? description)
    {
        ExpenseType = expenseType;
        Amount = amount;
        Description = description?.Trim();
    }

    public static CreditSaleExpense Create(ExpenseType expenseType, decimal amount, string? description = null)
    {
        if (amount < 0)
            throw new ArgumentException("Xərc məbləği mənfi ola bilməz", nameof(amount));

        if (description is not null && description.Length > SaleExpense.MaxDescriptionLength)
            throw new ArgumentException($"Təsvir maksimum {SaleExpense.MaxDescriptionLength} simvol ola bilər", nameof(description));

        return new CreditSaleExpense(expenseType, amount, description);
    }

    internal void AttachToCreditSale(Guid creditSaleId)
    {
        if (creditSaleId == Guid.Empty)
            throw new ArgumentException("Nisyə ID-si boş ola bilməz", nameof(creditSaleId));

        CreditSaleId = creditSaleId;
    }
}
