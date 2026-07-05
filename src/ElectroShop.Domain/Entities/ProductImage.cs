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
    public int DisplayOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    public string? ImageUrl { get; private set; }
    public string? PublicId { get; private set; }
    public string? ImagePath { get; private set; }
    public string? FileName { get; private set; }
    public string? ContentType { get; private set; }
    public long? Size { get; private set; }
    public string StorageProvider { get; private set; } = "Cloudinary";

    private ProductImage() { }

    private ProductImage(
        Guid productId,
        Guid imageId,
        int displayOrder,
        bool isPrimary,
        string? imageUrl,
        string? publicId,
        string? imagePath,
        string? fileName,
        string? contentType,
        long? size,
        string? storageProvider)
    {
        ProductId = productId;
        ImageId = imageId;
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
        ImageUrl = imageUrl;
        PublicId = publicId;
        ImagePath = imagePath;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        StorageProvider = storageProvider ?? "Cloudinary";
    }

    public static ProductImage Create(
        Guid productId,
        Guid imageId,
        int displayOrder,
        bool isPrimary = false,
        string? imageUrl = null,
        string? publicId = null,
        string? imagePath = null,
        string? fileName = null,
        string? contentType = null,
        long? size = null,
        string? storageProvider = null)
    {
        if (displayOrder < 0)
            throw new ArgumentException("Display order mənfi ola bilməz", nameof(displayOrder));

        return new ProductImage(
            productId,
            imageId,
            displayOrder,
            isPrimary,
            imageUrl,
            publicId,
            imagePath,
            fileName,
            contentType,
            size,
            storageProvider);
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

    public void SetStorageMetadata(
        string imageUrl,
        string publicId,
        string? fileName,
        string? contentType,
        long size,
        string storageProvider = "Cloudinary")
    {
        ImageUrl = imageUrl;
        PublicId = publicId;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        StorageProvider = storageProvider;
    }
}
