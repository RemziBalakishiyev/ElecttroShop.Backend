using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;

namespace ElectroShop.Application.Mappings;

/// <summary>
/// Mapster configuration for Brand entity mappings
/// </summary>
public class BrandMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Brand -> BrandDto
        config.NewConfig<Brand, BrandDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.CreatedAt, src => src.CreatedAtUtc);

        // Brand -> BrandListDto
        config.NewConfig<Brand, BrandListDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
    }
}

