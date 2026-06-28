using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetProductRatingSummary;

public class GetProductRatingSummaryQueryHandler : IRequestHandler<GetProductRatingSummaryQuery, Result<ProductRatingSummaryResponse>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IProductRatingQueryRepository _ratingRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetProductRatingSummaryQueryHandler(
        IProductQueryRepository productRepository,
        IProductRatingQueryRepository ratingRepository,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _ratingRepository = ratingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ProductRatingSummaryResponse>> Handle(
        GetProductRatingSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return DomainErrors.Product.NotFound(request.ProductId);

        var currentUserId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : null;
        var summary = await _ratingRepository.GetSummaryAsync(request.ProductId, currentUserId, cancellationToken);

        return Result.Success(new ProductRatingSummaryResponse
        {
            ProductId = request.ProductId,
            AverageRating = summary.AverageRating,
            RatingCount = summary.RatingCount,
            CurrentUserRating = summary.CurrentUserRating
        });
    }
}
