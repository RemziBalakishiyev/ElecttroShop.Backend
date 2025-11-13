using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;

namespace ElectroShop.Application.Mappings;

/// <summary>
/// Mapster configuration for Customer entity mappings
/// </summary>
public class CustomerMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Customer -> CustomerDto
        config.NewConfig<Customer, CustomerDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.CreatedAt, src => src.CreatedAtUtc)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAtUtc);
    }
}

