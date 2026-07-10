using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Filtering;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class CreditSaleQueryRepository : QueryRepository<CreditSale>, ICreditSaleQueryRepository
{
    public CreditSaleQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<(List<CreditSale> CreditSales, int TotalCount)> GetCreditSalesPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();
        var searchLower = search?.ToLower();
        var todayUtc = DateTime.UtcNow.Date;

        var predicate = PredicateBuilder.True<CreditSale>()
            .And(c => !c.IsDeleted)
            .AndIf(!string.IsNullOrWhiteSpace(searchLower), c =>
                (c.CustomerName != null && c.CustomerName.ToLower().Contains(searchLower!)) ||
                (c.CustomerPhone != null && c.CustomerPhone.ToLower().Contains(searchLower!)) ||
                c.ProductName.ToLower().Contains(searchLower!) ||
                (c.ProductCode != null && c.ProductCode.ToLower().Contains(searchLower!)))
            .AndIf(status.HasValue, c => c.Status == status!.Value)
            .AndIf(overdueOnly == true, c =>
                c.Status == CreditSaleStatus.Pending && c.DueDate.Date < todayUtc)
            .AndIf(productSource.HasValue, c => c.ProductSource == productSource!.Value)
            .AndIf(fromDate.HasValue, c => c.CreditDate >= fromDate!.Value)
            .AndIf(toDate.HasValue, c => c.CreditDate <= toDate!.Value)
            .AndIf(dueFromDate.HasValue, c => c.DueDate >= dueFromDate!.Value)
            .AndIf(dueToDate.HasValue, c => c.DueDate <= dueToDate!.Value);

        var filtered = query.Where(predicate);

        var totalCount = await filtered.CountAsync(cancellationToken);
        if (totalCount == 0)
            return (new List<CreditSale>(), 0);

        var items = await filtered
            .OrderByDescending(c => c.CreditDate)
            .ThenByDescending(c => c.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<CreditSale?> GetCreditSaleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ConvertedSale)
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<CreditSale?> GetCreditSaleForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<CreditSale?> GetCreditSaleWithExpensesForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var creditSale = await _dbSet.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (creditSale is null)
            return null;

        await _context.Entry(creditSale)
            .Collection(c => c.Expenses)
            .Query()
            .IgnoreQueryFilters()
            .LoadAsync(cancellationToken);

        return creditSale;
    }

    public async Task<CreditSaleSummaryAggregate> GetSummaryAsync(
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var todayUtc = DateTime.UtcNow.Date;
        var query = _dbSet.AsNoTracking().Where(c => !c.IsDeleted);

        if (fromDate.HasValue)
            query = query.Where(c => c.CreditDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(c => c.CreditDate <= toDate.Value);

        var items = await query.ToListAsync(cancellationToken);

        var pending = items.Where(c => c.Status == CreditSaleStatus.Pending).ToList();
        var overdue = pending.Where(c => c.DueDate.Date < todayUtc).ToList();
        var sold = items.Where(c => c.Status == CreditSaleStatus.Sold).ToList();
        var cancelled = items.Where(c => c.Status == CreditSaleStatus.Cancelled).ToList();

        return new CreditSaleSummaryAggregate
        {
            PendingCount = pending.Count,
            OverdueCount = overdue.Count,
            SoldCount = sold.Count,
            CancelledCount = cancelled.Count,
            TotalDebtAmount = pending.Sum(c => c.TotalSaleAmount),
            TotalPendingDebtAmount = pending.Where(c => c.DueDate.Date >= todayUtc).Sum(c => c.TotalSaleAmount),
            TotalOverdueDebtAmount = overdue.Sum(c => c.TotalSaleAmount),
            TotalSoldAmount = sold.Sum(c => c.TotalSaleAmount),
            TotalExpectedProfit = items
                .Where(c => c.Status == CreditSaleStatus.Pending || c.Status == CreditSaleStatus.Sold)
                .Sum(c => c.GrossProfit),
            TotalNetProfit = items
                .Where(c => c.Status == CreditSaleStatus.Pending || c.Status == CreditSaleStatus.Sold)
                .Sum(c => c.NetProfit)
        };
    }
}
