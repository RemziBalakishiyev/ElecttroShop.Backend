using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroShop.Persistence.Helpers;

public static class QueryHelper
{
    public static async Task<(List<T> Items, int TotalCount)> ExecutePagedAsync<T>(
        IQueryable<T> query,
        int page,
        int pageSize,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = true,
        CancellationToken cancellationToken = default) where T : class
    {
        var totalCount = await query.CountAsync(cancellationToken);
        
        if (totalCount == 0)
            return (new List<T>(), 0);

        if (orderBy != null)
        {
            query = descending 
                ? query.OrderByDescending(orderBy) 
                : query.OrderBy(orderBy);
        }
        try
        {

            var items1 = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            var ex = e;
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }
}

