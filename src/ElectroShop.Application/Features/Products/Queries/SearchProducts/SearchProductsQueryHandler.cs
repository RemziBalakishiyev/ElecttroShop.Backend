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
    private readonly IDiscountCalculationService _discountCalculationService;
    private readonly IImageStorage _imageStorage;

    public SearchProductsQueryHandler(
        IProductQueryRepository productRepository,
        IDiscountCalculationService discountCalculationService,
        IImageStorage imageStorage)
    {
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
        _imageStorage = imageStorage;
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
                var extension = await _imageStorage.GetImageExtensionAsync(primaryImage.ImageId, cancellationToken);
                primaryImageUrl = extension != null 
                    ? $"/api/images/{primaryImage.ImageId}{extension}" 
                    : $"/api/images/{primaryImage.ImageId}";
            }

            var productDto = product.Adapt<ProductListDto>();
            productDto = productDto with
            {
                FinalDiscountPercent = discountPercent,
                FinalPrice = finalPrice,
                PrimaryImageUrl = primaryImageUrl
            };

            productDtos.Add(productDto);
        }

        return PagedResult<ProductListDto>.Success(productDtos, request.Page, request.PageSize, totalCount);
    }
}

