using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Filtering;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class OrderQueryRepository : QueryRepository<Order>, IOrderQueryRepository
{
    public OrderQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<(List<Order> Orders, int TotalCount)> GetOrdersByCustomerPagedAsync(
        Guid customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .AsNoTracking();

        var predicate = PredicateBuilder.True<Order>()
            .And(o => o.CustomerId == customerId);

        return await QueryHelper.ExecutePagedAsync(
            query.Where(predicate),
            page,
            pageSize,
            o => o.CreatedAtUtc,
            descending: true,
            cancellationToken);
    }
}

