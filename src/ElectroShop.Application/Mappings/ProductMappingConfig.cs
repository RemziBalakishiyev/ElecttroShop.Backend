using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;

namespace ElectroShop.Application.Mappings;

/// <summary>
/// Mapster configuration for Product entity mappings
/// </summary>
public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Product -> ProductDto
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Price, src => src.Price.Amount)
            .Map(dest => dest.Currency, src => src.Price.Currency)
            .Map(dest => dest.Sku, src => src.Sku.Value)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty)
            .Map(dest => dest.BrandId, src => src.BrandId)
            .Map(dest => dest.BrandName, src => src.Brand != null ? src.Brand.Name : string.Empty)
            .Map(dest => dest.VatRate, src => src.VatRate)
            .Map(dest => dest.Stock, src => src.Stock)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.CreatedAt, src => src.CreatedAtUtc)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAtUtc);

        // Product -> ProductListDto
        config.NewConfig<Product, ProductListDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Price, src => src.Price.Amount)
            .Map(dest => dest.Currency, src => src.Price.Currency)
            .Map(dest => dest.Sku, src => src.Sku.Value)
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty)
            .Map(dest => dest.BrandName, src => src.Brand != null ? src.Brand.Name : string.Empty)
            .Map(dest => dest.Stock, src => src.Stock)
            .Map(dest => dest.IsActive, src => src.IsActive);

        // CreateProductDto -> Product (handled in command handler with value objects)
        // UpdateProductDto -> Product (handled in command handler with value objects)
    }
}

