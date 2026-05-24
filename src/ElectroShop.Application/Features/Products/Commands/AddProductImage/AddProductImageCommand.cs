using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.AddProductImage;

public record AddProductImageCommand : IRequest<Result<ProductImageDto>>
{
    public Guid ProductId { get; init; }
    public Guid ImageId { get; init; }
    public int DisplayOrder { get; init; } = 0;
    public bool IsPrimary { get; init; } = false;
}






