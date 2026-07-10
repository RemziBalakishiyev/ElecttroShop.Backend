using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.DTOs;

public record CreateCreditSaleRequest
{
    public string? CustomerName { get; init; }
    public string? CustomerPhone { get; init; }
    public CreditSaleProductSource ProductSourceType { get; init; }
    public Guid? ProductId { get; init; }
    public string? ProductName { get; init; }
    public string? Sku { get; init; }
    public decimal CostPrice { get; init; }
    public decimal SalePrice { get; init; }
    public int Quantity { get; init; }
    public IReadOnlyList<SaleExpenseRequestDto>? Expenses { get; init; }
    public DateTime CreditDate { get; init; }
    public DateTime DueDate { get; init; }
    public string? Note { get; init; }
}

public record UpdateCreditSaleRequest
{
    public string? CustomerName { get; init; }
    public string? CustomerPhone { get; init; }
    public decimal CostPrice { get; init; }
    public decimal SalePrice { get; init; }
    public int Quantity { get; init; }
    public IReadOnlyList<SaleExpenseRequestDto>? Expenses { get; init; }
    public DateTime CreditDate { get; init; }
    public DateTime DueDate { get; init; }
    public string? Note { get; init; }
}

public record MarkCreditSaleAsSoldRequest
{
    public DateTime? PaymentDate { get; init; }
    public DateTime? SoldDate { get; init; }
}

public record CreditSaleListItemDto
{
    public Guid Id { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerPhone { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public CreditSaleProductSource ProductSourceType { get; init; }
    public string ProductSourceTypeName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal CostPrice { get; init; }
    public decimal SalePrice { get; init; }
    public decimal TotalCostAmount { get; init; }
    public decimal TotalSaleAmount { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal GrossProfit { get; init; }
    public decimal NetProfit { get; init; }
    public DateTime CreditDate { get; init; }
    public DateTime DueDate { get; init; }
    public int DebtDurationDays { get; init; }
    public CreditSaleStatus Status { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public bool IsOverdue { get; init; }
    public int DaysLeft { get; init; }
    public Guid? ConvertedSaleId { get; init; }
    public DateTime? ConvertedAt { get; init; }
}

public record CreditSaleDetailDto : CreditSaleListItemDto
{
    public Guid? ProductId { get; init; }
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public string? Note { get; init; }
    public IReadOnlyList<SaleExpenseDto> Expenses { get; init; } = [];
    public DateTime? ConvertedSaleSoldAt { get; init; }
    public decimal? ConvertedSaleTotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? UpdatedBy { get; init; }
}

public record CreditSaleSummaryDto
{
    public int PendingCount { get; init; }
    public int OverdueCount { get; init; }
    public int SoldCount { get; init; }
    public int CancelledCount { get; init; }
    public decimal TotalDebtAmount { get; init; }
    public decimal TotalPendingDebtAmount { get; init; }
    public decimal TotalOverdueDebtAmount { get; init; }
    public decimal TotalSoldAmount { get; init; }
    public decimal TotalExpectedProfit { get; init; }
    public decimal TotalNetProfit { get; init; }
}
