using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

/// <summary>
/// Məhsulun şəkilləri üçün entity
/// Bir məhsulun bir neçə şəkili ola bilər
/// </summary>
public class ProductImage : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
    public Guid ImageId { get; private set; }
    public int DisplayOrder { get; private set; } // Şəkillərin sırası
    public bool IsPrimary { get; private set; } // Əsas şəkil

    private ProductImage() { }

    private ProductImage(Guid productId, Guid imageId, int displayOrder, bool isPrimary = false)
    {
        ProductId = productId;
        ImageId = imageId;
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
    }

    public static ProductImage Create(
        Guid productId,
        Guid imageId,
        int displayOrder,
        bool isPrimary = false)
    {
        if (displayOrder < 0)
            throw new ArgumentException("Display order mənfi ola bilməz", nameof(displayOrder));

        return new ProductImage(productId, imageId, displayOrder, isPrimary);
    }

    public void UpdateDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new ArgumentException("Display order mənfi ola bilməz", nameof(displayOrder));
        
        DisplayOrder = displayOrder;
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    public void RemoveAsPrimary()
    {
        IsPrimary = false;
    }
}


