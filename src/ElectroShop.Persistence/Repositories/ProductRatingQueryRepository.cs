using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class ProductRatingQueryRepository : QueryRepository<ProductRating>, IProductRatingQueryRepository
{
    public ProductRatingQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<ProductRating?> GetByProductAndUserAsync(
        Guid productId,
        Guid userId,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = includeDeleted
            ? _context.ProductRatings.IgnoreQueryFilters()
            : _dbSet.AsQueryable();

        return await query
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId, cancellationToken);
    }

    public async Task<(List<ProductRating> Ratings, int TotalCount)> GetRatingsPagedAsync(
        Guid productId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var ratings = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (ratings, totalCount);
    }

    public async Task<ProductRatingAggregateWithUserSummary> GetSummaryAsync(
        Guid productId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var ratingsQuery = _dbSet
            .AsNoTracking()
            .Where(r => r.ProductId == productId);

        var ratingCount = await ratingsQuery.CountAsync(cancellationToken);

        decimal averageRating = 0;
        if (ratingCount > 0)
        {
            var average = await ratingsQuery.AverageAsync(r => (double)r.RatingValue, cancellationToken);
            averageRating = Math.Round((decimal)average, 2, MidpointRounding.AwayFromZero);
        }

        int? currentUserRating = null;
        if (currentUserId.HasValue)
        {
            currentUserRating = await ratingsQuery
                .Where(r => r.UserId == currentUserId.Value)
                .Select(r => (int?)r.RatingValue)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new ProductRatingAggregateWithUserSummary(averageRating, ratingCount, currentUserRating);
    }

    public async Task<Dictionary<Guid, ProductRatingAggregateSummary>> GetSummariesByProductIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, ProductRatingAggregateSummary>();

        var aggregates = await _dbSet
            .AsNoTracking()
            .Where(r => ids.Contains(r.ProductId))
            .GroupBy(r => r.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                AverageRating = Math.Round(g.Average(r => (double)r.RatingValue), 2),
                RatingCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        return aggregates.ToDictionary(
            x => x.ProductId,
            x => new ProductRatingAggregateSummary((decimal)x.AverageRating, x.RatingCount));
    }

    public async Task<Dictionary<Guid, int>> GetCurrentUserRatingsByProductIdsAsync(
        IEnumerable<Guid> productIds,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, int>();

        return await _dbSet
            .AsNoTracking()
            .Where(r => ids.Contains(r.ProductId) && r.UserId == userId)
            .ToDictionaryAsync(r => r.ProductId, r => r.RatingValue, cancellationToken);
    }
}
