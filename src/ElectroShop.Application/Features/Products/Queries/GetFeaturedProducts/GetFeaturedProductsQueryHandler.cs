using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetFeaturedProducts;

public class GetFeaturedProductsQueryHandler : IRequestHandler<GetFeaturedProductsQuery, Result<List<ProductListDto>>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IDiscountCalculationService _discountCalculationService;
    private readonly IImageStorage _imageStorage;

    public GetFeaturedProductsQueryHandler(
        IProductQueryRepository productRepository,
        IDiscountCalculationService discountCalculationService,
        IImageStorage imageStorage)
    {
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
        _imageStorage = imageStorage;
    }

    public async Task<Result<List<ProductListDto>>> Handle(GetFeaturedProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetFeaturedProductsAsync(cancellationToken);

        if (products.Count == 0)
        {
            return Result.Success(new List<ProductListDto>());
        }

        var productDtos = new List<ProductListDto>();

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
                IsFeatured = true,
                DisplayOrder = product.DisplayOrder,
                PrimaryImageUrl = primaryImageUrl
            };

            productDtos.Add(productDto);
        }

        return Result.Success(productDtos);
    }
}

