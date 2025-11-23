using ElectroShop.Domain.Events;
using ElectroShop.Domain.Primitives;
using ElectroShop.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroShop.Domain.Entities;

public class Product : BaseCommonEntity
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
    public Guid? ImageId { get; private set; }

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
    /// Məhsulun şəkil ID-sini yenilə (DDD pattern)
    /// </summary>
    public void UpdateImageId(Guid? imageId)
    {
        ImageId = imageId;
    }
}