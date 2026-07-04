using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

/// <summary>
/// Məhsula aid atributlar/spesifikasiyalar (məsələn: Model, Ekran, Prosessor).
/// CategoryAttribute-dan fərqli olaraq bunlar yalnız bir məhsula aiddir,
/// kateqoriyadakı digər məhsullara təsir etmir.
/// </summary>
public class ProductAttribute : BaseCommonEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public string AttributeType { get; private set; } = default!;
    public bool IsRequired { get; private set; }
    public int DisplayOrder { get; private set; }
    public List<ProductAttributeValue> Values { get; private set; } = [];

    private ProductAttribute() { }

    private ProductAttribute(
        Guid productId,
        string name,
        string displayName,
        string attributeType,
        bool isRequired,
        int displayOrder)
    {
        ProductId = productId;
        Name = name;
        DisplayName = displayName;
        AttributeType = attributeType;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
    }

    public static ProductAttribute Create(
        Guid productId,
        string name,
        string displayName,
        string attributeType,
        bool isRequired = false,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Atribut adı boş ola bilməz", nameof(name));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display adı boş ola bilməz", nameof(displayName));

        if (string.IsNullOrWhiteSpace(attributeType))
            throw new ArgumentException("Atribut tipi boş ola bilməz", nameof(attributeType));

        return new ProductAttribute(productId, name, displayName, attributeType, isRequired, displayOrder);
    }

    public void AddValue(ProductAttributeValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Values.Add(value);
    }
}

/// <summary>
/// Məhsul atributunun dəyəri (məsələn: "Samsung Galaxy S25 Ultra", "6.9 düym").
/// </summary>
public class ProductAttributeValue : BaseEntity
{
    public const int MaxValueLength = 500;

    public Guid ProductAttributeId { get; private set; }
    public ProductAttribute ProductAttribute { get; private set; } = default!;
    public string Value { get; private set; } = default!;
    public string? DisplayValue { get; private set; }
    public int DisplayOrder { get; private set; }
    public string? ColorCode { get; private set; }

    private ProductAttributeValue() { }

    private ProductAttributeValue(
        Guid productAttributeId,
        string value,
        string? displayValue,
        int displayOrder,
        string? colorCode)
    {
        ProductAttributeId = productAttributeId;
        Value = value;
        DisplayValue = displayValue;
        DisplayOrder = displayOrder;
        ColorCode = colorCode;
    }

    public static ProductAttributeValue Create(
        Guid productAttributeId,
        string value,
        string? displayValue = null,
        int displayOrder = 0,
        string? colorCode = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dəyər boş ola bilməz", nameof(value));

        if (value.Length > MaxValueLength)
            throw new ArgumentException($"Dəyər maksimum {MaxValueLength} simvol ola bilər", nameof(value));

        return new ProductAttributeValue(productAttributeId, value, displayValue ?? value, displayOrder, colorCode);
    }
}

/// <summary>
/// Məhsul atributunu yaratmaq üçün input (Application layer-dən domain-ə ötürülür).
/// </summary>
public sealed record ProductAttributeDraft(
    string Name,
    string DisplayName,
    string AttributeType,
    bool IsRequired,
    int DisplayOrder,
    IReadOnlyList<ProductAttributeValueDraft> Values);

public sealed record ProductAttributeValueDraft(
    string Value,
    string? DisplayValue,
    int DisplayOrder,
    string? ColorCode);
