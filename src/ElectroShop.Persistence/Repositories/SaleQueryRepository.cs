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
            .AndIf(maxProfit.HasValue, s => s.Profit <= maxProfit!.Value);

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
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
