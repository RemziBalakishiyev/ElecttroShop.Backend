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

    public GetFeaturedProductsQueryHandler(
        IProductQueryRepository productRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
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

            var productDto = product.Adapt<ProductListDto>();
            productDto = productDto with
            {
                FinalDiscountPercent = discountPercent,
                FinalPrice = finalPrice,
                IsFeatured = true,
                DisplayOrder = product.DisplayOrder
            };

            productDtos.Add(productDto);
        }

        return Result.Success(productDtos);
    }
}

