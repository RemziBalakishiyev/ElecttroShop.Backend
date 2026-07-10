using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Sales.Common;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.Features.CreditSales.Common;

public static class CreditSaleMapper
{
    public static CreditSaleListItemDto ToListItemDto(CreditSale creditSale, DateTime? todayUtc = null)
    {
        var today = (todayUtc ?? DateTime.UtcNow).Date;
        var isOverdue = creditSale.Status == CreditSaleStatus.Pending && creditSale.DueDate.Date < today;
        var daysLeft = (creditSale.DueDate.Date - today).Days;

        return new CreditSaleListItemDto
        {
            Id = creditSale.Id,
            CustomerName = creditSale.CustomerName,
            CustomerPhone = creditSale.CustomerPhone,
            ProductName = creditSale.ProductName,
            Sku = creditSale.ProductCode,
            ProductSourceType = creditSale.ProductSource,
            ProductSourceTypeName = GetProductSourceName(creditSale.ProductSource),
            Quantity = creditSale.Quantity,
            CostPrice = creditSale.CostPrice,
            SalePrice = creditSale.SalePrice,
            TotalCostAmount = creditSale.TotalCostAmount,
            TotalSaleAmount = creditSale.TotalSaleAmount,
            TotalExpenses = creditSale.TotalExpenses,
            GrossProfit = creditSale.GrossProfit,
            NetProfit = creditSale.NetProfit,
            CreditDate = creditSale.CreditDate,
            DueDate = creditSale.DueDate,
            DebtDurationDays = creditSale.DebtDurationDays,
            Status = creditSale.Status,
            StatusName = GetStatusName(creditSale.Status, isOverdue),
            IsOverdue = isOverdue,
            DaysLeft = daysLeft,
            ConvertedSaleId = creditSale.ConvertedSaleId,
            ConvertedAt = creditSale.ConvertedAt
        };
    }

    public static CreditSaleDetailDto ToDetailDto(CreditSale creditSale, DateTime? todayUtc = null)
    {
        var listItem = ToListItemDto(creditSale, todayUtc);

        return new CreditSaleDetailDto
        {
            Id = listItem.Id,
            CustomerName = listItem.CustomerName,
            CustomerPhone = listItem.CustomerPhone,
            ProductName = listItem.ProductName,
            Sku = listItem.Sku,
            ProductSourceType = listItem.ProductSourceType,
            ProductSourceTypeName = listItem.ProductSourceTypeName,
            Quantity = listItem.Quantity,
            CostPrice = listItem.CostPrice,
            SalePrice = listItem.SalePrice,
            TotalCostAmount = listItem.TotalCostAmount,
            TotalSaleAmount = listItem.TotalSaleAmount,
            TotalExpenses = listItem.TotalExpenses,
            GrossProfit = listItem.GrossProfit,
            NetProfit = listItem.NetProfit,
            CreditDate = listItem.CreditDate,
            DueDate = listItem.DueDate,
            DebtDurationDays = listItem.DebtDurationDays,
            Status = listItem.Status,
            StatusName = listItem.StatusName,
            IsOverdue = listItem.IsOverdue,
            DaysLeft = listItem.DaysLeft,
            ConvertedSaleId = listItem.ConvertedSaleId,
            ConvertedAt = listItem.ConvertedAt,
            ProductId = creditSale.ProductId,
            CategoryId = creditSale.CategoryId,
            CategoryName = creditSale.CategoryName,
            Note = creditSale.Note,
            Expenses = creditSale.Expenses
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.CreatedAtUtc)
                .Select(ToExpenseDto)
                .ToList(),
            ConvertedSaleSoldAt = creditSale.ConvertedSale?.SoldAt,
            ConvertedSaleTotalAmount = creditSale.ConvertedSale?.TotalSaleAmount,
            CreatedAt = creditSale.CreatedAtUtc,
            UpdatedAt = creditSale.UpdatedAtUtc,
            CreatedBy = creditSale.CreatedBy,
            UpdatedBy = creditSale.UpdatedBy
        };
    }

    public static SaleExpenseDto ToExpenseDto(CreditSaleExpense expense) => new()
    {
        Id = expense.Id,
        ExpenseType = expense.ExpenseType,
        Description = expense.Description,
        Amount = expense.Amount,
        CreatedAt = expense.CreatedAtUtc
    };

    public static IReadOnlyList<SaleExpenseDraft> ToExpenseDrafts(
        IReadOnlyList<SaleExpenseRequestDto>? expenses) =>
        SaleMapper.ToExpenseDrafts(expenses);

    public static string GetProductSourceName(CreditSaleProductSource source) => source switch
    {
        CreditSaleProductSource.Manual => "Manual",
        CreditSaleProductSource.SystemProduct => "Sistem məhsulu",
        _ => source.ToString()
    };

    public static string GetStatusName(CreditSaleStatus status, bool isOverdue) => status switch
    {
        CreditSaleStatus.Pending when isOverdue => "Vaxtı keçib",
        CreditSaleStatus.Pending => "Gözləyir",
        CreditSaleStatus.Sold => "Satılıb",
        CreditSaleStatus.Cancelled => "Ləğv edilib",
        _ => status.ToString()
    };
}
