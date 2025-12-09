using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class DiscountQueryRepository : QueryRepository<Discount>, IDiscountQueryRepository
{
    public DiscountQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<(List<Discount> Discounts, int TotalCount)> GetDiscountsPagedAsync(
        int page,
        int pageSize,
        DiscountType? type = null,
        bool? isActive = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(d => d.Product)
            .Include(d => d.Brand)
            .Include(d => d.Category)
            .AsNoTracking()
            .AsQueryable();

        // Filtering
        if (type.HasValue)
        {
            query = query.Where(d => d.Type == type.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(d => d.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();
            query = query.Where(d =>
                (d.Product != null && d.Product.Name.ToLower().Contains(searchTermLower)) ||
                (d.Brand != null && d.Brand.Name.ToLower().Contains(searchTermLower)) ||
                (d.Category != null && d.Category.Name.ToLower().Contains(searchTermLower)));
        }

        return await QueryHelper.ExecutePagedAsync(
            query,
            page,
            pageSize,
            d => d.CreatedAtUtc,
            descending: true,
            cancellationToken);
    }

    public async Task<Discount?> GetDiscountWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Product)
            .Include(d => d.Brand)
            .Include(d => d.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}





