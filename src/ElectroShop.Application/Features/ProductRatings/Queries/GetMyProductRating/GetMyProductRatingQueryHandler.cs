using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetMyProductRating;

public class GetMyProductRatingQueryHandler : IRequestHandler<GetMyProductRatingQuery, Result<ProductRatingResponse>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IProductRatingQueryRepository _ratingRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProductRatingQueryHandler(
        IProductQueryRepository productRepository,
        IProductRatingQueryRepository ratingRepository,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _ratingRepository = ratingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ProductRatingResponse>> Handle(
        GetMyProductRatingQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
            return Result.Failure<ProductRatingResponse>(DomainErrors.Authentication.Unauthorized);

        var userId = _currentUserService.UserId.Value;

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return DomainErrors.Product.NotFound(request.ProductId);

        var rating = await _ratingRepository.GetByProductAndUserAsync(
            request.ProductId,
            userId,
            cancellationToken: cancellationToken);

        if (rating is null)
            return DomainErrors.ProductRating.NotFound(request.ProductId);

        return Result.Success(rating.Adapt<ProductRatingResponse>());
    }
}
