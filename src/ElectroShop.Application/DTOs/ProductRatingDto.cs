namespace ElectroShop.Application.DTOs;

public record CreateProductRatingRequest
{
    public int RatingValue { get; init; }
    public string? Comment { get; init; }
}

public record UpdateProductRatingRequest
{
    public int RatingValue { get; init; }
    public string? Comment { get; init; }
}

public record ProductRatingResponse
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public Guid UserId { get; init; }
    public string? UserFullName { get; init; }
    public int RatingValue { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record ProductRatingSummaryResponse
{
    public Guid ProductId { get; init; }
    public decimal AverageRating { get; init; }
    public int RatingCount { get; init; }
    public int? CurrentUserRating { get; init; }
}
