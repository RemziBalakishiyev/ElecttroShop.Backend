using ElectroShop.Domain.Primitives;
using ElectroShop.Domain.ValueObjects;

namespace ElectroShop.Domain.Entities;

/// <summary>
/// Məhsul variantları - müxtəlif atribut kombinasiyaları
/// Məs: iPhone 12 - Black, iPhone 12 - White, iPhone 12 - Blue
/// Variantlar sadəcə atribut fərqlərini təmsil edir
/// SKU, Price və Stock Product səviyyəsindədir
/// </summary>
public class ProductVariant : BaseCommonEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public Guid? ImageId { get; private set; } // Variant üçün xüsusi şəkil (məs: rəngə görə)

    // Variant atributları (JSON formatında saxlanacaq)
    // Məs: {"Storage": "256GB", "Color": "Black", "RAM": "6GB"}
    public string AttributesJson { get; private set; } = "{}";

    private ProductVariant() { }

    private ProductVariant(
        Guid productId,
        string attributesJson,
        Guid? imageId = null)
    {
        ProductId = productId;
        AttributesJson = attributesJson;
        ImageId = imageId;
    }

    public static ProductVariant Create(
        Guid productId,
        string attributesJson,
        Guid? imageId = null)
    {
        if (string.IsNullOrWhiteSpace(attributesJson))
            throw new ArgumentException("Atributlar boş ola bilməz", nameof(attributesJson));

        return new ProductVariant(productId, attributesJson, imageId);
    }

    public void Update(
        string attributesJson,
        Guid? imageId = null)
    {
        if (string.IsNullOrWhiteSpace(attributesJson))
            throw new ArgumentException("Atributlar boş ola bilməz", nameof(attributesJson));

        AttributesJson = attributesJson;
        ImageId = imageId;
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



