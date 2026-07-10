using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.Abstractions;

public interface ICreditSaleQueryRepository : IQueryRepository<CreditSale>
{
    Task<(List<CreditSale> CreditSales, int TotalCount)> GetCreditSalesPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        CreditSaleStatus? status = null,
        bool? overdueOnly = null,
        CreditSaleProductSource? productSource = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        DateTime? dueFromDate = null,
        DateTime? dueToDate = null,
        CancellationToken cancellationToken = default);

    Task<CreditSale?> GetCreditSaleByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CreditSale?> GetCreditSaleForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CreditSale?> GetCreditSaleWithExpensesForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CreditSaleSummaryAggregate> GetSummaryAsync(
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);
}

public record CreditSaleSummaryAggregate
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
