using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetProductRatings;

public record GetProductRatingsQuery : IRequest<PagedResult<ProductRatingResponse>>
{
    public Guid ProductId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
