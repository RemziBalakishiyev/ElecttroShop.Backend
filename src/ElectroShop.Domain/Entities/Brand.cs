using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class Brand : BaseCommonEntity
{
    public string Name { get; private set; } = default!;
    public bool IsPromotional { get; private set; } = false;
    public int? DisplayOrder { get; private set; }

    private Brand() { }

    private Brand(string name)
    {
        Name = name;
    }

    public static Brand Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Brend adı boş ola bilməz", nameof(name));

        return new Brand(name.Trim());
    }

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Brend adı boş ola bilməz", nameof(name));

        Name = name.Trim();
    }

    public void SetPromotional(bool isPromotional, int? displayOrder = null)
    {
        IsPromotional = isPromotional;
        DisplayOrder = displayOrder;
    }
}

