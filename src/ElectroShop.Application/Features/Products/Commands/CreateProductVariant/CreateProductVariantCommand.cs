using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.CreateProductVariant;

public record CreateProductVariantCommand : IRequest<Result<ProductVariantDto>>
{
    public Guid ProductId { get; init; }
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "AZN";
    public int Stock { get; init; }
    public Guid? ImageId { get; init; }
    public Dictionary<string, string> Attributes { get; init; } = new();
}


