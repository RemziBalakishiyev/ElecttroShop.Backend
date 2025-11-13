using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class Category : BaseCommonEntity
{
    public string Name { get; private set; } = default!;
    public string? Slug { get; private set; }
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; }
    public List<Category> Children { get; private set; } = [];

    private Category() { }

    private Category(string name, string? slug, Guid? parentId = null)
    {
        Name = name;
        Slug = slug ?? GenerateSlug(name);
        ParentId = parentId;
    }

    public static Category Create(string name, Guid? parentId = null, string? customSlug = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Kateqoriya adı boş ola bilməz", nameof(name));

        var slug = customSlug ?? GenerateSlug(name);
        return new Category(name, slug, parentId);
    }

    public void Update(string name, Guid? parentId = null, string? customSlug = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Kateqoriya adı boş ola bilməz", nameof(name));

        Name = name;
        Slug = customSlug ?? GenerateSlug(name);
        ParentId = parentId;
    }

    private static string GenerateSlug(string name)
    {
        return name
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("ə", "e")
            .Replace("ö", "o")
            .Replace("ü", "u")
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ç", "c")
            .Replace("ş", "s");
    }
}

