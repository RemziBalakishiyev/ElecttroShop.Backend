using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;

namespace ElectroShop.Application.Mappings;

/// <summary>
/// Mapster configuration for User entity mappings
/// </summary>
public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // User -> UserDto
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.Role, src => src.Role)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.CreatedAt, src => src.CreatedAtUtc);
    }
}

