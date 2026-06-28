using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;

namespace ElectroShop.Application.Mappings;

public class ProductRatingMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProductRating, ProductRatingResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.UserFullName, src => src.User != null ? src.User.FullName : null)
            .Map(dest => dest.RatingValue, src => src.RatingValue)
            .Map(dest => dest.Comment, src => src.Comment)
            .Map(dest => dest.CreatedAt, src => src.CreatedAtUtc)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAtUtc);
    }
}
