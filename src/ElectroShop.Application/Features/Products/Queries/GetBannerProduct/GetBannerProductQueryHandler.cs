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

    public GetBannerProductQueryHandler(
        IProductQueryRepository productRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
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

        var productDto = product.Adapt<ProductDto>();
        productDto = productDto with
        {
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice,
            IsBanner = true,
            IsFeatured = product.IsFeatured,
            DisplayOrder = product.DisplayOrder
        };

        return Result.Success(productDto);
    }
}

