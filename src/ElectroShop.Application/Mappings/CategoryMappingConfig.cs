using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;

namespace ElectroShop.Application.Mappings;

/// <summary>
/// Mapster configuration for Category entity mappings
/// </summary>
public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Category -> CategoryDto
        config.NewConfig<Category, CategoryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.ParentId, src => src.ParentId)
            .Map(dest => dest.ParentName, src => src.Parent != null ? src.Parent.Name : null)
            .Map(dest => dest.CreatedAt, src => src.CreatedAtUtc);

        // Category -> CategoryListDto
        config.NewConfig<Category, CategoryListDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug);
    }
}

