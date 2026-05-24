using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Məhsulu yeniləmək üçün Command
/// Result pattern ilə ProductDto qaytarır
/// </summary>
public record UpdateProductCommand : IRequest<Result<ProductDto>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "TRY";
    public Guid CategoryId { get; init; }
    public Guid BrandId { get; init; }
    public decimal VatRate { get; init; } = 0.18m;
    public int Stock { get; init; }
    public List<Guid> ImageIds { get; init; } = [];
    public List<UpdateProductVariantDto> Variants { get; init; } = [];
    /// <summary>
    /// RowVersion - Optimistic Concurrency Control üçün
    /// Client-dən gəlir, DB-dəki RowVersion ilə müqayisə edilir
    /// </summary>
    public uint RowVersion { get; init; }
}

public record UpdateProductVariantDto
{
    public Guid? Id { get; init; } // Null olarsa yeni variant yaradılacaq
    public Guid? ImageId { get; init; }
    public Dictionary<string, string> Attributes { get; init; } = new();
    public bool IsActive { get; init; } = true;
}

