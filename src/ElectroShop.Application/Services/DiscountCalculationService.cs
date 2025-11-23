using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.Services;

/// <summary>
/// Endirim hesablama servisi implementasiyası
/// Prioritet: Məhsul > Brand > Kateqoriya
/// </summary>
public class DiscountCalculationService : IDiscountCalculationService
{
    private readonly IQueryRepository<Discount> _discountRepository;

    public DiscountCalculationService(IQueryRepository<Discount> discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<decimal> CalculateFinalDiscountPercentAsync(
        Guid productId,
        Guid categoryId,
        Guid brandId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Prioritet sırası ilə endirimləri yükləyirik
        // 1. Məhsula xüsusi endirim (ən yüksək prioritet)
        var productDiscount = await _discountRepository
            .FirstOrDefaultAsync(d =>
                d.Type == DiscountType.Product &&
                d.ProductId == productId &&
                d.IsActive &&
                d.StartDate <= now &&
                (d.EndDate == null || d.EndDate >= now),
                cancellationToken);

        if (productDiscount != null)
        {
            return productDiscount.Percent;
        }

        // 2. Brend endirimi
        var brandDiscount = await _discountRepository
            .FirstOrDefaultAsync(d =>
                d.Type == DiscountType.Brand &&
                d.BrandId == brandId &&
                d.IsActive &&
                d.StartDate <= now &&
                (d.EndDate == null || d.EndDate >= now),
                cancellationToken);

        if (brandDiscount != null)
        {
            return brandDiscount.Percent;
        }

        // 3. Kateqoriya endirimi (ən aşağı prioritet)
        var categoryDiscount = await _discountRepository
            .FirstOrDefaultAsync(d =>
                d.Type == DiscountType.Category &&
                d.CategoryId == categoryId &&
                d.IsActive &&
                d.StartDate <= now &&
                (d.EndDate == null || d.EndDate >= now),
                cancellationToken);

        if (categoryDiscount != null)
        {
            return categoryDiscount.Percent;
        }

        // Endirim yoxdursa 0 qaytarırıq
        return 0;
    }

    public decimal CalculateDiscountedPrice(decimal originalPrice, decimal discountPercent)
    {
        if (discountPercent <= 0)
            return originalPrice;

        if (discountPercent >= 100)
            return 0;

        var discountAmount = originalPrice * (discountPercent / 100);
        return originalPrice - discountAmount;
    }

    public async Task<decimal> GetCategoryDiscountPercentAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var categoryDiscount = await _discountRepository
            .FirstOrDefaultAsync(d =>
                d.Type == DiscountType.Category &&
                d.CategoryId == categoryId &&
                d.IsActive &&
                d.StartDate <= now &&
                (d.EndDate == null || d.EndDate >= now),
                cancellationToken);

        return categoryDiscount?.Percent ?? 0;
    }

    public async Task<decimal> GetBrandDiscountPercentAsync(
        Guid brandId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var brandDiscount = await _discountRepository
            .FirstOrDefaultAsync(d =>
                d.Type == DiscountType.Brand &&
                d.BrandId == brandId &&
                d.IsActive &&
                d.StartDate <= now &&
                (d.EndDate == null || d.EndDate >= now),
                cancellationToken);

        return brandDiscount?.Percent ?? 0;
    }
}

