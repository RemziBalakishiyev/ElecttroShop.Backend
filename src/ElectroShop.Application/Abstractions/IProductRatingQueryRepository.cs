using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Abstractions;

public record ProductRatingAggregateSummary(
    decimal AverageRating,
    int RatingCount);

public record ProductRatingAggregateWithUserSummary(
    decimal AverageRating,
    int RatingCount,
    int? CurrentUserRating);

public interface IProductRatingQueryRepository : IQueryRepository<ProductRating>
{
    Task<ProductRating?> GetByProductAndUserAsync(
        Guid productId,
        Guid userId,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    Task<(List<ProductRating> Ratings, int TotalCount)> GetRatingsPagedAsync(
        Guid productId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductRatingAggregateWithUserSummary> GetSummaryAsync(
        Guid productId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, ProductRatingAggregateSummary>> GetSummariesByProductIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetCurrentUserRatingsByProductIdsAsync(
        IEnumerable<Guid> productIds,
        Guid userId,
        CancellationToken cancellationToken = default);
}
