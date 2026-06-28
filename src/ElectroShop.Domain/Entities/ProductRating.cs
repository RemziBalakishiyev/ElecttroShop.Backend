using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class ProductRating : BaseCommonEntity
{
    public const int MinRatingValue = 1;
    public const int MaxRatingValue = 5;
    public const int MaxCommentLength = 2000;

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public int RatingValue { get; private set; }
    public string? Comment { get; private set; }

    private ProductRating() { }

    public static ProductRating Create(Guid productId, Guid userId, int ratingValue, string? comment = null)
    {
        ValidateRatingValue(ratingValue);
        ValidateComment(comment);

        if (productId == Guid.Empty)
            throw new ArgumentException("Məhsul ID-si boş ola bilməz", nameof(productId));

        if (userId == Guid.Empty)
            throw new ArgumentException("İstifadəçi ID-si boş ola bilməz", nameof(userId));

        return new ProductRating
        {
            ProductId = productId,
            UserId = userId,
            RatingValue = ratingValue,
            Comment = NormalizeComment(comment)
        };
    }

    public void Update(int ratingValue, string? comment = null)
    {
        ValidateRatingValue(ratingValue);
        ValidateComment(comment);

        RatingValue = ratingValue;
        Comment = NormalizeComment(comment);
    }

    public void Restore(int ratingValue, string? comment = null)
    {
        IsDeleted = false;
        Update(ratingValue, comment);
    }

    private static void ValidateRatingValue(int ratingValue)
    {
        if (ratingValue < MinRatingValue || ratingValue > MaxRatingValue)
            throw new ArgumentException($"Reytinq {MinRatingValue} ilə {MaxRatingValue} arasında olmalıdır", nameof(ratingValue));
    }

    private static void ValidateComment(string? comment)
    {
        if (comment is not null && comment.Length > MaxCommentLength)
            throw new ArgumentException($"Şərh maksimum {MaxCommentLength} simvol ola bilər", nameof(comment));
    }

    private static string? NormalizeComment(string? comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            return null;

        return comment.Trim();
    }
}
