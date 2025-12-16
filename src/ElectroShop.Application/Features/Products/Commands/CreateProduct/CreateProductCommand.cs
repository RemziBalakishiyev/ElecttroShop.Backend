using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Command to create a new product
/// Returns Result pattern with ProductDto
/// </summary>
public record CreateProductCommand : IRequest<Result<ProductDto>>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "TRY";
    public string Sku { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public Guid BrandId { get; init; }
    public decimal VatRate { get; init; } = 0.18m;
    public int Stock { get; init; }
    public List<Guid> ImageIds { get; init; } = [];
    public List<CreateProductVariantDto> Variants { get; init; } = [];
}

public record CreateProductVariantDto
{
    public Guid? ImageId { get; init; }
    public Dictionary<string, string> Attributes { get; init; } = new();
}

