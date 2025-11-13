using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetProducts;

/// <summary>
/// Səhifələnmiş məhsul siyahısı üçün Query
/// PagedResult pattern ilə məhsulları qaytarır
/// </summary>
public record GetProductsQuery : IRequest<PagedResult<ProductListDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? BrandId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? IsActive { get; init; }
}

