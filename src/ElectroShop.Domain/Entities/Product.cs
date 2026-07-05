using ElectroShop.Domain.Events;
using ElectroShop.Domain.Primitives;
using ElectroShop.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroShop.Domain.Entities;

public class Product : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public Sku Sku { get; private set; } = new("UNSET");
    public string? Description { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;

    public Guid BrandId { get; private set; }
    public Brand Brand { get; private set; } = default!;

    public Money Price { get; private set; } = new(0m, "AZN");
    public decimal VatRate { get; private set; } = 0.18m;
    public int Stock { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsBanner { get; private set; } = false;
    public bool IsFeatured { get; private set; } = false;
    public int? DisplayOrder { get; private set; }
    public bool IsPopular { get; private set; } = false;
    public int? PopularDisplayOrder { get; private set; }
    
    // Navigation properties
    public List<ProductImage> ProductImages { get; private set; } = [];
    public List<ProductVariant> ProductVariants { get; private set; } = [];
    public List<ProductAttribute> ProductAttributes { get; private set; } = [];

    private Product() { }

    private Product(string name, Sku sku, Guid categoryId, Guid brandId, Money price, decimal vatRate, int stock, string? description = null)
    {
        Name = name;
        Sku = sku;
        CategoryId = categoryId;
        BrandId = brandId;
        Price = price;
        VatRate = vatRate;
        Stock = stock;
        Description = description;
    }

    /// <summary>
    /// Factory method to create a Product (DDD pattern)
    /// Encapsulates domain logic and value object creation
    /// </summary>
    public static Product Create(
        string name,
        string sku,
        Guid categoryId,
        Guid brandId,
        decimal price,
        string currency,
        decimal vatRate,
        int stock,
        string? description = null)
    {
        // Create value objects (will throw if invalid)
        var skuValueObject = new Sku(sku);
        var priceValueObject = new Money(price, currency);

        return new Product(name, skuValueObject, categoryId, brandId, priceValueObject, vatRate, stock, description);
    }

    /// <summary>
    /// Məhsul məlumatlarını yenilə (DDD pattern)
    /// </summary>
    public void Update(
        string name,
        string? description,
        decimal price,
        string currency,
        Guid categoryId,
        Guid brandId,
        decimal vatRate,
        int stock)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        BrandId = brandId;
        VatRate = vatRate;
        Stock = stock;

        // Qiymət dəyişibsə, domain event yarat
        if (price != Price.Amount || currency != Price.Currency)
        {
            var oldPrice = Price.Amount;
            Price = new Money(price, currency);
            AddDomainEvent(new ProductPriceChanged(Id, oldPrice, price));
        }
    }

    /// <summary>
    /// Qiyməti dəyiş (domain event ilə)
    /// </summary>
    public void ChangePrice(decimal newAmount)
    {
        if (newAmount != Price.Amount)
        {
            var oldPrice = Price.Amount;
            Price = new Money(newAmount, Price.Currency);
            AddDomainEvent(new ProductPriceChanged(Id, oldPrice, newAmount));
        }
    }

    /// <summary>
    /// Stoku azalt
    /// </summary>
    public void DecreaseStock(int qty)
    {
        if (qty <= 0) 
            throw new ArgumentException("Miqdar müsbət olmalıdır", nameof(qty));
        
        if (Stock < qty) 
            throw new InvalidOperationException("Stokda kifayət qədər məhsul yoxdur");
        
        Stock -= qty;
    }

    /// <summary>
    /// Stoku artır
    /// </summary>
    public void IncreaseStock(int qty)
    {
        if (qty <= 0) 
            throw new ArgumentException("Miqdar müsbət olmalıdır", nameof(qty));
        
        Stock += qty;
    }

    /// <summary>
    /// Məhsulu deaktiv et (soft delete)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Məhsulu aktiv et
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Məhsula şəkil əlavə et
    /// </summary>
    public void AddImage(
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
        var productImage = ProductImage.Create(
            Id,
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

        ProductImages.Add(productImage);
    }

    /// <summary>
    /// Məhsuldan şəkil sil
    /// </summary>
    public void RemoveImage(Guid imageId)
    {
        var image = ProductImages.FirstOrDefault(img => img.ImageId == imageId);
        if (image != null)
        {
            ProductImages.Remove(image);
        }
    }

    /// <summary>
    /// Əsas şəkili təyin et
    /// </summary>
    public void SetPrimaryImage(Guid imageId)
    {
        foreach (var image in ProductImages)
        {
            if (image.ImageId == imageId)
            {
                image.SetAsPrimary();
            }
            else
            {
                image.RemoveAsPrimary();
            }
        }
    }

    /// <summary>
    /// Məhsulu Banner olaraq təyin et
    /// </summary>
    public void SetAsBanner()
    {
        IsBanner = true;
    }

    /// <summary>
    /// Məhsulu Banner-dan çıxar
    /// </summary>
    public void RemoveFromBanner()
    {
        IsBanner = false;
    }

    /// <summary>
    /// Məhsulu Featured olaraq təyin et
    /// </summary>
    public void SetAsFeatured(int displayOrder)
    {
        if (displayOrder < 1 || displayOrder > 5)
            throw new ArgumentException("Display order 1-5 arasında olmalıdır", nameof(displayOrder));
        
        IsFeatured = true;
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Məhsulu Featured-dan çıxar
    /// </summary>
    public void RemoveFromFeatured()
    {
        IsFeatured = false;
        DisplayOrder = null;
    }

    /// <summary>
    /// Featured məhsulun display order-ini yenilə
    /// </summary>
    public void UpdateDisplayOrder(int displayOrder)
    {
        if (!IsFeatured)
            throw new InvalidOperationException("Yalnız featured məhsulların display order-i yenilənə bilər");
        
        if (displayOrder < 1 || displayOrder > 5)
            throw new ArgumentException("Display order 1-5 arasında olmalıdır", nameof(displayOrder));
        
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Məhsulu Popular olaraq təyin et (ana səhifə "Məşhur Məhsullar" bölməsi)
    /// </summary>
    public void SetAsPopular(int displayOrder)
    {
        if (displayOrder < 1 || displayOrder > 4)
            throw new ArgumentException("Popular display order 1-4 arasında olmalıdır", nameof(displayOrder));

        IsPopular = true;
        PopularDisplayOrder = displayOrder;
    }

    /// <summary>
    /// Məhsulu Popular-dan çıxar
    /// </summary>
    public void RemoveFromPopular()
    {
        IsPopular = false;
        PopularDisplayOrder = null;
    }

    /// <summary>
    /// Məhsul məlumatlarını yenilə (UpdateDetails - daha aydın ad)
    /// </summary>
    public void UpdateDetails(
        string name,
        string? description,
        decimal price,
        string currency,
        Guid categoryId,
        Guid brandId,
        decimal vatRate,
        int stock)
    {
        Update(name, description, price, currency, categoryId, brandId, vatRate, stock);
    }

    /// <summary>
    /// Şəkilləri sinxronizasiya et (DDD Aggregate pattern)
    /// Yalnız aggregate root vasitəsilə child entity-lər dəyişdirilir
    /// </summary>
    public void SyncImages(List<Guid> imageIds)
    {
        if (imageIds == null)
            throw new ArgumentNullException(nameof(imageIds));

        // Silinməli şəkillər
        var imagesToRemove = ProductImages
            .Where(x => !imageIds.Contains(x.ImageId))
            .ToList();

        foreach (var image in imagesToRemove)
        {
            ProductImages.Remove(image);
        }

        for (int i = 0; i < imageIds.Count; i++)
        {
            var imageId = imageIds[i];
            var existingImage = ProductImages.FirstOrDefault(img => img.ImageId == imageId);

            if (existingImage is null)
            {
                AddImage(imageId, i, isPrimary: i == 0);
            }
            else
            {
                existingImage.UpdateDisplayOrder(i);
                if (i == 0)
                    SetPrimaryImage(imageId);
            }
        }
    }

    /// <summary>
    /// Məhsulun atributlarını (spesifikasiyalarını) tam əvəz et (DDD Aggregate pattern).
    /// Bu atributlar yalnız bu məhsula aiddir, kateqoriyaya təsir etmir.
    /// </summary>
    public void SyncAttributes(IReadOnlyList<ProductAttributeDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        // Tam əvəzləmə: köhnə atributları sil, yenilərini əlavə et
        ProductAttributes.Clear();

        foreach (var draft in drafts)
        {
            var attribute = ProductAttribute.Create(
                Id,
                draft.Name,
                draft.DisplayName,
                draft.AttributeType,
                draft.IsRequired,
                draft.DisplayOrder);

            foreach (var valueDraft in draft.Values)
            {
                var value = ProductAttributeValue.Create(
                    attribute.Id,
                    valueDraft.Value,
                    valueDraft.DisplayValue,
                    valueDraft.DisplayOrder,
                    valueDraft.ColorCode);

                attribute.AddValue(value);
            }

            ProductAttributes.Add(attribute);
        }
    }

    /// <summary>
    /// Variantları sinxronizasiya et (DDD Aggregate pattern)
    /// Yalnız aggregate root vasitəsilə child entity-lər dəyişdirilir
    /// </summary>
    public void SyncVariants(
        List<(Guid? Id, string AttributesJson, Guid? ImageId, bool IsActive)> variants)
    {
        if (variants == null)
            throw new ArgumentNullException(nameof(variants));

        var existingVariantIds = ProductVariants.Select(v => v.Id).ToList();

        // Yeni variantlar
        foreach (var variantData in variants.Where(v => !v.Id.HasValue))
        {
            if (string.IsNullOrWhiteSpace(variantData.AttributesJson))
                throw new ArgumentException("Variant atributları boş ola bilməz", nameof(variants));

            var variant = ProductVariant.Create(
                Id,
                variantData.AttributesJson,
                variantData.ImageId
            );

            if (!variantData.IsActive)
                variant.Deactivate();

            ProductVariants.Add(variant);
        }

        // Mövcud variantları yenilə
        foreach (var variantData in variants.Where(v => v.Id.HasValue))
        {
            var variantId = variantData.Id!.Value;
            var variant = ProductVariants.FirstOrDefault(v => v.Id == variantId);
            
            if (variant == null)
                throw new InvalidOperationException($"Variant tapılmadı: {variantId}");

            if (string.IsNullOrWhiteSpace(variantData.AttributesJson))
                throw new ArgumentException("Variant atributları boş ola bilməz", nameof(variants));

            variant.Update(variantData.AttributesJson, variantData.ImageId);

            if (variantData.IsActive)
                variant.Activate();
            else
                variant.Deactivate();
        }
    }

    /// <summary>
    /// Verilmiş siyahıda olmayan variantları deaktiv et
    /// </summary>
    public void DeactivateMissingVariants(List<Guid> activeVariantIds)
    {
        if (activeVariantIds == null)
            throw new ArgumentNullException(nameof(activeVariantIds));

        var variantsToDeactivate = ProductVariants
            .Where(v => !activeVariantIds.Contains(v.Id))
            .ToList();

        foreach (var variant in variantsToDeactivate)
        {
            variant.Deactivate();
        }
    }

    /// <summary>
    /// Variant əlavə et (DDD Aggregate pattern)
    /// </summary>
    public ProductVariant AddVariant(string attributesJson, Guid? imageId = null)
    {
        if (string.IsNullOrWhiteSpace(attributesJson))
            throw new ArgumentException("Variant atributları boş ola bilməz", nameof(attributesJson));

        var variant = ProductVariant.Create(Id, attributesJson, imageId);
        ProductVariants.Add(variant);
        return variant;
    }

    /// <summary>
    /// Variant yenilə (DDD Aggregate pattern)
    /// </summary>
    public void UpdateVariant(Guid variantId, string attributesJson, Guid? imageId = null, bool? isActive = null)
    {
        var variant = ProductVariants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            throw new InvalidOperationException($"Variant tapılmadı: {variantId}");

        if (string.IsNullOrWhiteSpace(attributesJson))
            throw new ArgumentException("Variant atributları boş ola bilməz", nameof(attributesJson));

        variant.Update(attributesJson, imageId);

        if (isActive.HasValue)
        {
            if (isActive.Value)
                variant.Activate();
            else
                variant.Deactivate();
        }
    }

    /// <summary>
    /// Variant sil (deaktiv et) (DDD Aggregate pattern)
    /// </summary>
    public void RemoveVariant(Guid variantId)
    {
        var variant = ProductVariants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            throw new InvalidOperationException($"Variant tapılmadı: {variantId}");

        variant.Deactivate();
    }
}