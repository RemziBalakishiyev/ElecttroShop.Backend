namespace ElectroShop.Application.Services;

/// <summary>
/// Endirim hesablama servisi
/// Prioritet: Məhsul > Brand > Kateqoriya
/// </summary>
public interface IDiscountCalculationService
{
    /// <summary>
    /// Məhsul üçün final endirim faizini hesablayır
    /// </summary>
    /// <param name="productId">Məhsul ID-si</param>
    /// <param name="categoryId">Kateqoriya ID-si</param>
    /// <param name="brandId">Brend ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Final endirim faizi (0-100 arası)</returns>
    Task<decimal> CalculateFinalDiscountPercentAsync(
        Guid productId,
        Guid categoryId,
        Guid brandId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Endirimli qiyməti hesablayır
    /// </summary>
    /// <param name="originalPrice">Orijinal qiymət</param>
    /// <param name="discountPercent">Endirim faizi</param>
    /// <returns>Endirimli qiymət</returns>
    decimal CalculateDiscountedPrice(decimal originalPrice, decimal discountPercent);

    /// <summary>
    /// Kateqoriya üçün endirim faizini əldə edir
    /// </summary>
    /// <param name="categoryId">Kateqoriya ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Endirim faizi (0-100 arası)</returns>
    Task<decimal> GetCategoryDiscountPercentAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Brend üçün endirim faizini əldə edir
    /// </summary>
    /// <param name="brandId">Brend ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Endirim faizi (0-100 arası)</returns>
    Task<decimal> GetBrandDiscountPercentAsync(
        Guid brandId,
        CancellationToken cancellationToken = default);
}

