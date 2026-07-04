using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

/// <summary>
/// Kateqoriyaya görə atributlar (məsələn: Telefon üçün - Yaddaş, Rəng, RAM və s.)
/// Hər kateqoriyanın öz atributları ola bilər
/// </summary>
public class CategoryAttribute : BaseCommonEntity
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;
    public string Name { get; private set; } = default!; // Məs: "Yaddaş", "Rəng", "RAM"
    public string DisplayName { get; private set; } = default!; // Məs: "Yaddaş seçin", "Rəng seçin"
    public string AttributeType { get; private set; } = default!; // "Storage", "Color", "RAM", "Screen", etc.
    public bool IsRequired { get; private set; } = false;
    public int DisplayOrder { get; private set; }
    public List<CategoryAttributeValue> Values { get; private set; } = []; // Məs: "128GB", "256GB", "512GB"

    private CategoryAttribute() { }

    private CategoryAttribute(
        Guid categoryId,
        string name,
        string displayName,
        string attributeType,
        bool isRequired,
        int displayOrder)
    {
        CategoryId = categoryId;
        Name = name;
        DisplayName = displayName;
        AttributeType = attributeType;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
    }

    public static CategoryAttribute Create(
        Guid categoryId,
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

        return new CategoryAttribute(categoryId, name, displayName, attributeType, isRequired, displayOrder);
    }

    public void Update(
        string name,
        string displayName,
        string attributeType,
        bool isRequired,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Atribut adı boş ola bilməz", nameof(name));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display adı boş ola bilməz", nameof(displayName));

        if (string.IsNullOrWhiteSpace(attributeType))
            throw new ArgumentException("Atribut tipi boş ola bilməz", nameof(attributeType));

        Name = name;
        DisplayName = displayName;
        AttributeType = attributeType;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
    }

    public void AddValue(CategoryAttributeValue value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (Values.Any(v => v.Value == value.Value))
            throw new InvalidOperationException($"Bu atribut dəyəri artıq mövcuddur: {value.Value}");

        Values.Add(value);
    }

    public void RemoveValue(CategoryAttributeValue value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        Values.Remove(value);
    }
}

/// <summary>
/// Kateqoriya atributunun mümkün dəyərləri
/// Məs: Yaddaş atributu üçün: "128GB", "256GB", "512GB", "1TB"
/// </summary>
public class CategoryAttributeValue : BaseEntity
{
    public const int MaxValueLength = 500;

    public Guid CategoryAttributeId { get; private set; }
    public CategoryAttribute CategoryAttribute { get; private set; } = default!;
    public string Value { get; private set; } = default!; // Məs: "128GB", "Black", "6GB"
    public string? DisplayValue { get; private set; } // Məs: "128 GB", "Qara", "6 GB RAM"
    public int DisplayOrder { get; private set; }
    public string? ColorCode { get; private set; } // Rəng atributları üçün hex kod (məs: "#000000")

    private CategoryAttributeValue() { }

    private CategoryAttributeValue(
        Guid categoryAttributeId,
        string value,
        string? displayValue,
        int displayOrder,
        string? colorCode = null)
    {
        CategoryAttributeId = categoryAttributeId;
        Value = value;
        DisplayValue = displayValue;
        DisplayOrder = displayOrder;
        ColorCode = colorCode;
    }

    public static CategoryAttributeValue Create(
        Guid categoryAttributeId,
        string value,
        string? displayValue = null,
        int displayOrder = 0,
        string? colorCode = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dəyər boş ola bilməz", nameof(value));

        if (value.Length > MaxValueLength)
            throw new ArgumentException($"Dəyər maksimum {MaxValueLength} simvol ola bilər", nameof(value));

        return new CategoryAttributeValue(categoryAttributeId, value, displayValue ?? value, displayOrder, colorCode);
    }

    public void Update(
        string value,
        string? displayValue,
        int displayOrder,
        string? colorCode = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dəyər boş ola bilməz", nameof(value));

        if (value.Length > MaxValueLength)
            throw new ArgumentException($"Dəyər maksimum {MaxValueLength} simvol ola bilər", nameof(value));

        Value = value;
        DisplayValue = displayValue ?? value;
        DisplayOrder = displayOrder;
        ColorCode = colorCode;
    }
}






