using ElectroShop.Domain.Primitives;
using ElectroShop.Domain.ValueObjects;

namespace ElectroShop.Domain.Entities;

/// <summary>
/// Məhsul variantları - müxtəlif atribut kombinasiyaları
/// Məs: iPhone 14 Pro - 256GB Black, iPhone 14 Pro - 512GB White
/// Hər variantın öz qiyməti, stoku və SKU-su ola bilər
/// </summary>
public class ProductVariant : BaseCommonEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
    public Sku Sku { get; private set; } = new("UNSET");
    public Money Price { get; private set; } = new(0m, "AZN");
    public int Stock { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid? ImageId { get; private set; } // Variant üçün xüsusi şəkil (məs: rəngə görə)

    // Variant atributları (JSON formatında saxlanacaq)
    // Məs: {"Storage": "256GB", "Color": "Black", "RAM": "6GB"}
    public string AttributesJson { get; private set; } = "{}";

    private ProductVariant() { }

    private ProductVariant(
        Guid productId,
        Sku sku,
        Money price,
        int stock,
        string attributesJson,
        Guid? imageId = null)
    {
        ProductId = productId;
        Sku = sku;
        Price = price;
        Stock = stock;
        AttributesJson = attributesJson;
        ImageId = imageId;
    }

    public static ProductVariant Create(
        Guid productId,
        string sku,
        decimal price,
        string currency,
        int stock,
        string attributesJson,
        Guid? imageId = null)
    {
        if (string.IsNullOrWhiteSpace(attributesJson))
            throw new ArgumentException("Atributlar boş ola bilməz", nameof(attributesJson));

        var skuValueObject = new Sku(sku);
        var priceValueObject = new Money(price, currency);

        return new ProductVariant(productId, skuValueObject, priceValueObject, stock, attributesJson, imageId);
    }

    public void Update(
        string sku,
        decimal price,
        string currency,
        int stock,
        string attributesJson,
        Guid? imageId = null)
    {
        if (string.IsNullOrWhiteSpace(attributesJson))
            throw new ArgumentException("Atributlar boş ola bilməz", nameof(attributesJson));

        Sku = new Sku(sku);
        Price = new Money(price, currency);
        Stock = stock;
        AttributesJson = attributesJson;
        ImageId = imageId;
    }

    public void DecreaseStock(int qty)
    {
        if (qty <= 0)
            throw new ArgumentException("Miqdar müsbət olmalıdır", nameof(qty));

        if (Stock < qty)
            throw new InvalidOperationException("Stokda kifayət qədər məhsul yoxdur");

        Stock -= qty;
    }

    public void IncreaseStock(int qty)
    {
        if (qty <= 0)
            throw new ArgumentException("Miqdar müsbət olmalıdır", nameof(qty));

        Stock += qty;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateImageId(Guid? imageId)
    {
        ImageId = imageId;
    }
}


