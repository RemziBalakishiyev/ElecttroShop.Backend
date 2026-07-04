using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PagedResult<ProductListDto>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IProductRatingQueryRepository _ratingRepository;
    private readonly IDiscountCalculationService _discountCalculationService;
    private readonly IImageUrlResolver _imageUrlResolver;

    public SearchProductsQueryHandler(
        IProductQueryRepository productRepository,
        IProductRatingQueryRepository ratingRepository,
        IDiscountCalculationService discountCalculationService,
        IImageUrlResolver imageUrlResolver)
    {
        _productRepository = productRepository;
        _ratingRepository = ratingRepository;
        _discountCalculationService = discountCalculationService;
        _imageUrlResolver = imageUrlResolver;
    }

    public async Task<PagedResult<ProductListDto>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetProductsPagedAsync(
            request.Page,
            request.PageSize,
            searchTerm: request.SearchTerm,
            cancellationToken: cancellationToken);

        if (totalCount == 0)
        {
            return PagedResult<ProductListDto>.Empty(request.Page, request.PageSize);
        }

        var productIds = products.Select(p => p.Id).ToList();
        var ratingSummaries = await _ratingRepository.GetSummariesByProductIdsAsync(productIds, cancellationToken);

        var productDtos = new List<ProductListDto>();

        // Hər məhsul üçün endirim hesabla
        foreach (var product in products)
        {
            var discountPercent = await _discountCalculationService.CalculateFinalDiscountPercentAsync(
                product.Id,
                product.CategoryId,
                product.BrandId,
                cancellationToken);

            var finalPrice = _discountCalculationService.CalculateDiscountedPrice(
                product.Price.Amount,
                discountPercent);

            // PrimaryImageUrl-i set et
            var primaryImage = product.ProductImages
                .OrderBy(pi => pi.IsPrimary ? 0 : 1)
                .ThenBy(pi => pi.DisplayOrder)
                .FirstOrDefault();
            
            string? primaryImageUrl = null;
            if (primaryImage != null)
            {
                primaryImageUrl = await _imageUrlResolver.BuildImageUrlAsync(primaryImage.ImageId, cancellationToken);
            }

            var productDto = product.Adapt<ProductListDto>();
            productDto = productDto with
            {
                FinalDiscountPercent = discountPercent,
                FinalPrice = finalPrice,
                PrimaryImageUrl = primaryImageUrl,
                AverageRating = ratingSummaries.TryGetValue(product.Id, out var summary) ? summary.AverageRating : 0,
                RatingCount = ratingSummaries.TryGetValue(product.Id, out var countSummary) ? countSummary.RatingCount : 0
            };

            productDtos.Add(productDto);
        }

        return PagedResult<ProductListDto>.Success(productDtos, request.Page, request.PageSize, totalCount);
    }
}

