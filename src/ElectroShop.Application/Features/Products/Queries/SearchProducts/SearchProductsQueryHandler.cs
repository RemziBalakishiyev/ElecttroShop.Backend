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

    public SearchProductsQueryHandler(
        IProductQueryRepository productRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
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

            var productDto = product.Adapt<ProductListDto>();
            productDto = productDto with
            {
                FinalDiscountPercent = discountPercent,
                FinalPrice = finalPrice
            };

            productDtos.Add(productDto);
        }

        return PagedResult<ProductListDto>.Success(productDtos, request.Page, request.PageSize, totalCount);
    }
}

