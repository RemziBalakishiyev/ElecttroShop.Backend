using ElectroShop.Domain.Enums;
using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

/// <summary>
/// Endirim entity-si
/// </summary>
public class Discount : BaseCommonEntity
{
    public DiscountType Type { get; private set; }
    public Guid? ProductId { get; private set; }
    public Product? Product { get; private set; }
    public Guid? BrandId { get; private set; }
    public Brand? Brand { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public decimal Percent { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Discount() { }

    private Discount(
        DiscountType type,
        decimal percent,
        DateTime startDate,
        DateTime? endDate = null,
        Guid? productId = null,
        Guid? brandId = null,
        Guid? categoryId = null,
        bool isActive = true)
    {
        Type = type;
        Percent = percent;
        StartDate = startDate;
        EndDate = endDate;
        ProductId = productId;
        BrandId = brandId;
        CategoryId = categoryId;
        IsActive = isActive;
    }

    /// <summary>
    /// Məhsula xüsusi endirim yaradır
    /// </summary>
    public static Discount CreateProductDiscount(
        Guid productId,
        decimal percent,
        DateTime startDate,
        DateTime? endDate = null,
        bool isActive = true)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Məhsul ID-si boş ola bilməz", nameof(productId));

        if (percent < 0 || percent > 100)
            throw new ArgumentException("Endirim faizi 0-100 arasında olmalıdır", nameof(percent));

        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("Bitmə tarixi başlanğıc tarixindən sonra olmalıdır", nameof(endDate));

        return new Discount(DiscountType.Product, percent, startDate, endDate, productId: productId, isActive: isActive);
    }

    /// <summary>
    /// Brend endirimi yaradır
    /// </summary>
    public static Discount CreateBrandDiscount(
        Guid brandId,
        decimal percent,
        DateTime startDate,
        DateTime? endDate = null,
        bool isActive = true)
    {
        if (brandId == Guid.Empty)
            throw new ArgumentException("Brend ID-si boş ola bilməz", nameof(brandId));

        if (percent < 0 || percent > 100)
            throw new ArgumentException("Endirim faizi 0-100 arasında olmalıdır", nameof(percent));

        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("Bitmə tarixi başlanğıc tarixindən sonra olmalıdır", nameof(endDate));

        return new Discount(DiscountType.Brand, percent, startDate, endDate, brandId: brandId, isActive: isActive);
    }

    /// <summary>
    /// Kateqoriya endirimi yaradır
    /// </summary>
    public static Discount CreateCategoryDiscount(
        Guid categoryId,
        decimal percent,
        DateTime startDate,
        DateTime? endDate = null,
        bool isActive = true)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Kateqoriya ID-si boş ola bilməz", nameof(categoryId));

        if (percent < 0 || percent > 100)
            throw new ArgumentException("Endirim faizi 0-100 arasında olmalıdır", nameof(percent));

        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("Bitmə tarixi başlanğıc tarixindən sonra olmalıdır", nameof(endDate));

        return new Discount(DiscountType.Category, percent, startDate, endDate, categoryId: categoryId, isActive: isActive);
    }

    /// <summary>
    /// Endirimi yeniləyir
    /// </summary>
    public void Update(
        decimal percent,
        DateTime startDate,
        DateTime? endDate = null)
    {
        if (percent < 0 || percent > 100)
            throw new ArgumentException("Endirim faizi 0-100 arasında olmalıdır", nameof(percent));

        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("Bitmə tarixi başlanğıc tarixindən sonra olmalıdır", nameof(endDate));

        Percent = percent;
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Endirimin aktiv olub-olmadığını yoxlayır (tarix və IsActive əsasında)
    /// </summary>
    public bool IsCurrentlyActive(DateTime? checkDate = null)
    {
        if (!IsActive)
            return false;

        var date = checkDate ?? DateTime.UtcNow;

        if (date < StartDate)
            return false;

        if (EndDate.HasValue && date > EndDate.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Endirimi deaktiv edir
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Endirimi aktiv edir
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }
}

