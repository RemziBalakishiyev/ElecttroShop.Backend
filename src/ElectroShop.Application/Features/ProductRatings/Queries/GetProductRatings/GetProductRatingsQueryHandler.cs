using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetProductRatings;

public class GetProductRatingsQueryHandler : IRequestHandler<GetProductRatingsQuery, PagedResult<ProductRatingResponse>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IProductRatingQueryRepository _ratingRepository;

    public GetProductRatingsQueryHandler(
        IProductQueryRepository productRepository,
        IProductRatingQueryRepository ratingRepository)
    {
        _productRepository = productRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task<PagedResult<ProductRatingResponse>> Handle(
        GetProductRatingsQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return PagedResult<ProductRatingResponse>.Failure(DomainErrors.Product.NotFound(request.ProductId));

        var (ratings, totalCount) = await _ratingRepository.GetRatingsPagedAsync(
            request.ProductId,
            request.Page,
            request.PageSize,
            cancellationToken);

        if (totalCount == 0)
            return PagedResult<ProductRatingResponse>.Empty(request.Page, request.PageSize);

        var ratingDtos = ratings.Adapt<List<ProductRatingResponse>>();
        return PagedResult<ProductRatingResponse>.Success(ratingDtos, request.Page, request.PageSize, totalCount);
    }
}
