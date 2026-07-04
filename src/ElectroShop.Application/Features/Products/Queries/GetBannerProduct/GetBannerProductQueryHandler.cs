using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetBannerProduct;

public class GetBannerProductQueryHandler : IRequestHandler<GetBannerProductQuery, Result<ProductDto>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IDiscountCalculationService _discountCalculationService;
    private readonly IImageUrlResolver _imageUrlResolver;

    public GetBannerProductQueryHandler(
        IProductQueryRepository productRepository,
        IDiscountCalculationService discountCalculationService,
        IImageUrlResolver imageUrlResolver)
    {
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
        _imageUrlResolver = imageUrlResolver;
    }

    public async Task<Result<ProductDto>> Handle(GetBannerProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetBannerProductAsync(cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductDto>(Error.NotFound(
                "Product.BannerNotFound",
                "Banner məhsul tapılmadı."));
        }

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

        var productDto = product.Adapt<ProductDto>();
        productDto = productDto with
        {
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice,
            IsBanner = true,
            IsFeatured = product.IsFeatured,
            DisplayOrder = product.DisplayOrder,
            PrimaryImageUrl = primaryImageUrl
        };

        return Result.Success(productDto);
    }
}

