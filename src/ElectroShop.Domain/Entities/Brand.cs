using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class Brand : BaseCommonEntity
{
    public string Name { get; private set; } = default!;

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
}

