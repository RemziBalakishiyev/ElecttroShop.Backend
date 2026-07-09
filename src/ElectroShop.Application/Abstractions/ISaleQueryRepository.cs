using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.Abstractions;

public interface ISaleQueryRepository : IQueryRepository<Sale>
{
    Task<(List<Sale> Sales, int TotalCount)> GetSalesPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        Guid? categoryId = null,
        Guid? productId = null,
        SaleSource? saleSource = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        decimal? minProfit = null,
        decimal? maxProfit = null,
        decimal? minExpense = null,
        decimal? maxExpense = null,
        CancellationToken cancellationToken = default);

    Task<Sale?> GetSaleByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Sale?> GetSaleWithExpensesForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilmiş tarix intervalı üzrə satış statistikaları (SoldAt əsasında, UTC)
    /// </summary>
    Task<SalesStatisticsDto> GetSalesStatisticsAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilmiş tarix intervalı üzrə satış siyahısı (SoldAt əsasında, UTC, end exclusive)
    /// </summary>
    Task<List<Sale>> GetSalesBySoldAtRangeAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Satış statistikaları (repository aggregation)
/// </summary>
public record SalesStatisticsDto
{
    public decimal TotalSaleAmount { get; init; }
    public decimal TotalProductCost { get; init; }
    public decimal TotalExpenses { get; init; }
    public int SoldProductQuantity { get; init; }
    public int SalesCount { get; init; }

    public decimal TotalProfit => TotalSaleAmount - TotalProductCost - TotalExpenses;
}
