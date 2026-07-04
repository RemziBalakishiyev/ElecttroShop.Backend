using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Features.Sales.Common;

internal static class SaleMapper
{
    public static SaleListItemDto ToListItemDto(Sale sale) => new()
    {
        Id = sale.Id,
        ProductId = sale.ProductId,
        ProductName = sale.ProductName,
        ProductCode = sale.ProductCode,
        CategoryId = sale.CategoryId,
        CategoryName = sale.CategoryName,
        CostPrice = sale.CostPrice,
        SalePrice = sale.SalePrice,
        Quantity = sale.Quantity,
        TotalCost = sale.TotalCost,
        TotalSaleAmount = sale.TotalSaleAmount,
        TotalExpenses = sale.TotalExpenses,
        Profit = sale.Profit,
        SaleSource = sale.SaleSource,
        SoldAt = sale.SoldAt,
        Note = sale.Note,
        CreatedAt = sale.CreatedAtUtc
    };

    public static SaleDetailDto ToDetailDto(Sale sale) => new()
    {
        Id = sale.Id,
        ProductId = sale.ProductId,
        ProductName = sale.ProductName,
        ProductCode = sale.ProductCode,
        CategoryId = sale.CategoryId,
        CategoryName = sale.CategoryName,
        CostPrice = sale.CostPrice,
        SalePrice = sale.SalePrice,
        Quantity = sale.Quantity,
        TotalCost = sale.TotalCost,
        TotalSaleAmount = sale.TotalSaleAmount,
        TotalExpenses = sale.TotalExpenses,
        Profit = sale.Profit,
        SaleSource = sale.SaleSource,
        SoldAt = sale.SoldAt,
        Note = sale.Note,
        CreatedAt = sale.CreatedAtUtc,
        Expenses = sale.Expenses
            .Where(e => !e.IsDeleted)
            .OrderBy(e => e.CreatedAtUtc)
            .Select(ToExpenseDto)
            .ToList(),
        UpdatedAt = sale.UpdatedAtUtc,
        CreatedBy = sale.CreatedBy,
        UpdatedBy = sale.UpdatedBy
    };

    public static SaleExpenseDto ToExpenseDto(SaleExpense expense) => new()
    {
        Id = expense.Id,
        ExpenseType = expense.ExpenseType,
        Description = expense.Description,
        Amount = expense.Amount,
        CreatedAt = expense.CreatedAtUtc
    };

    public static IReadOnlyList<SaleExpenseDraft> ToExpenseDrafts(
        IReadOnlyList<SaleExpenseRequestDto>? expenses) =>
        expenses?.Select(e => new SaleExpenseDraft(e.ExpenseType, e.Amount, e.Description)).ToList()
        ?? [];
}
