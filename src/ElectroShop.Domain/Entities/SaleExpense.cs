using ElectroShop.Domain.Enums;
using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class SaleExpense : BaseCommonEntity
{
    public const int MaxDescriptionLength = 1000;

    public Guid SaleId { get; private set; }
    public Sale Sale { get; private set; } = default!;
    public ExpenseType ExpenseType { get; private set; }
    public string? Description { get; private set; }
    public decimal Amount { get; private set; }

    private SaleExpense() { }

    private SaleExpense(ExpenseType expenseType, decimal amount, string? description)
    {
        ExpenseType = expenseType;
        Amount = amount;
        Description = description?.Trim();
    }

    public static SaleExpense Create(ExpenseType expenseType, decimal amount, string? description = null)
    {
        if (amount < 0)
            throw new ArgumentException("Xərc məbləği mənfi ola bilməz", nameof(amount));

        if (description is not null && description.Length > MaxDescriptionLength)
            throw new ArgumentException($"Təsvir maksimum {MaxDescriptionLength} simvol ola bilər", nameof(description));

        return new SaleExpense(expenseType, amount, description);
    }

    internal void AttachToSale(Guid saleId)
    {
        if (saleId == Guid.Empty)
            throw new ArgumentException("Satış ID-si boş ola bilməz", nameof(saleId));

        SaleId = saleId;
    }
}

public sealed record SaleExpenseDraft(
    ExpenseType ExpenseType,
    decimal Amount,
    string? Description);
