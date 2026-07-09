using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Filtering;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class SaleQueryRepository : QueryRepository<Sale>, ISaleQueryRepository
{
    private const string UncategorizedCategoryName = "Kateqoriyasız";

    public SaleQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<(List<Sale> Sales, int TotalCount)> GetSalesPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();
        var searchLower = search?.ToLower();

        var predicate = PredicateBuilder.True<Sale>()
            .And(s => !s.IsDeleted)
            .AndIf(!string.IsNullOrWhiteSpace(searchLower), s =>
                s.ProductName.ToLower().Contains(searchLower!) ||
                (s.ProductCode != null && s.ProductCode.ToLower().Contains(searchLower!)) ||
                (s.CategoryName != null && s.CategoryName.ToLower().Contains(searchLower!)))
            .AndIf(categoryId.HasValue, s => s.CategoryId == categoryId!.Value)
            .AndIf(productId.HasValue, s => s.ProductId == productId!.Value)
            .AndIf(saleSource.HasValue, s => s.SaleSource == saleSource!.Value)
            .AndIf(dateFrom.HasValue, s => s.SoldAt >= dateFrom!.Value)
            .AndIf(dateTo.HasValue, s => s.SoldAt <= dateTo!.Value)
            .AndIf(minProfit.HasValue, s => s.Profit >= minProfit!.Value)
            .AndIf(maxProfit.HasValue, s => s.Profit <= maxProfit!.Value)
            .AndIf(minExpense.HasValue, s => s.TotalExpenses >= minExpense!.Value)
            .AndIf(maxExpense.HasValue, s => s.TotalExpenses <= maxExpense!.Value);

        return await QueryHelper.ExecutePagedAsync(
            query.Where(predicate),
            page,
            pageSize,
            s => s.SoldAt,
            descending: true,
            cancellationToken);
    }

    public async Task<Sale?> GetSaleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.Expenses)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetSaleWithExpensesForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _dbSet
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sale is null)
            return null;

        await _context.Entry(sale)
            .Collection(s => s.Expenses)
            .Query()
            .IgnoreQueryFilters()
            .LoadAsync(cancellationToken);

        return sale;
    }

    public async Task<SalesStatisticsDto> GetSalesStatisticsAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        CancellationToken cancellationToken = default)
    {
        var stats = await _dbSet
            .AsNoTracking()
            .Where(s => s.SoldAt >= dateFromUtc && s.SoldAt < dateToUtcExclusive)
            .GroupBy(_ => 1)
            .Select(g => new SalesStatisticsDto
            {
                TotalSaleAmount = g.Sum(s => s.TotalSaleAmount),
                TotalProductCost = g.Sum(s => s.TotalCost),
                TotalExpenses = g.Sum(s => s.TotalExpenses),
                SoldProductQuantity = g.Sum(s => s.Quantity),
                SalesCount = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return stats ?? new SalesStatisticsDto();
    }

    public async Task<List<Sale>> GetSalesBySoldAtRangeAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => !s.IsDeleted
                && s.SoldAt >= dateFromUtc
                && s.SoldAt < dateToUtcExclusive)
            .OrderByDescending(s => s.SoldAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DailySalesAggregateDto>> GetDailySalesAggregatesAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.SoldAt >= dateFromUtc && s.SoldAt < dateToUtcExclusive)
            .GroupBy(s => s.SoldAt.Date)
            .Select(g => new DailySalesAggregateDto
            {
                Date = g.Key,
                SalesCount = g.Count(),
                TotalSalesAmount = g.Sum(s => s.TotalSaleAmount),
                TotalCostAmount = g.Sum(s => s.TotalCost),
                TotalExpenses = g.Sum(s => s.TotalExpenses)
            })
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TopProductAggregateDto>> GetTopProductsAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.SoldAt >= dateFromUtc && s.SoldAt < dateToUtcExclusive)
            .GroupBy(s => new { s.ProductName, Sku = s.ProductCode, s.CategoryName })
            .Select(g => new TopProductAggregateDto
            {
                ProductName = g.Key.ProductName,
                Sku = g.Key.Sku,
                CategoryName = g.Key.CategoryName,
                Quantity = g.Sum(s => s.Quantity),
                TotalSalesAmount = g.Sum(s => s.TotalSaleAmount),
                TotalProfit = g.Sum(s => s.Profit)
            })
            .OrderByDescending(p => p.Quantity)
            .ThenByDescending(p => p.TotalSalesAmount)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CategorySalesAggregateDto>> GetCategorySalesAggregatesAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.SoldAt >= dateFromUtc && s.SoldAt < dateToUtcExclusive)
            .GroupBy(s => s.CategoryName ?? UncategorizedCategoryName)
            .Select(g => new CategorySalesAggregateDto
            {
                CategoryName = g.Key,
                SalesCount = g.Count(),
                Quantity = g.Sum(s => s.Quantity),
                TotalSalesAmount = g.Sum(s => s.TotalSaleAmount),
                TotalProfit = g.Sum(s => s.Profit)
            })
            .OrderByDescending(c => c.TotalSalesAmount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SaleTypeAggregateDto>> GetSaleTypeAggregatesAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.SoldAt >= dateFromUtc && s.SoldAt < dateToUtcExclusive)
            .GroupBy(s => s.SaleSource)
            .Select(g => new SaleTypeAggregateDto
            {
                SaleSource = g.Key,
                SalesCount = g.Count(),
                TotalSalesAmount = g.Sum(s => s.TotalSaleAmount),
                TotalProfit = g.Sum(s => s.Profit)
            })
            .OrderByDescending(s => s.TotalSalesAmount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductProfitAggregateDto>> GetProductProfitAggregatesAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.SoldAt >= dateFromUtc && s.SoldAt < dateToUtcExclusive)
            .GroupBy(s => new { s.ProductName, Sku = s.ProductCode })
            .Select(g => new ProductProfitAggregateDto
            {
                ProductName = g.Key.ProductName,
                Sku = g.Key.Sku,
                TotalSalesAmount = g.Sum(s => s.TotalSaleAmount),
                TotalCostAmount = g.Sum(s => s.TotalCost),
                TotalExpenses = g.Sum(s => s.TotalExpenses),
                NetProfit = g.Sum(s => s.Profit)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RecentSaleAggregateDto>> GetRecentSalesAsync(
        DateTime dateFromUtc,
        DateTime dateToUtcExclusive,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.SoldAt >= dateFromUtc && s.SoldAt < dateToUtcExclusive)
            .OrderByDescending(s => s.SoldAt)
            .Take(limit)
            .Select(s => new RecentSaleAggregateDto
            {
                ProductName = s.ProductName,
                ProductCode = s.ProductCode,
                CategoryName = s.CategoryName,
                SaleSource = s.SaleSource,
                SalePrice = s.SalePrice,
                Quantity = s.Quantity,
                TotalCost = s.TotalCost,
                TotalSaleAmount = s.TotalSaleAmount,
                TotalExpenses = s.TotalExpenses,
                Profit = s.Profit,
                SoldAt = s.SoldAt
            })
            .ToListAsync(cancellationToken);
    }
}
