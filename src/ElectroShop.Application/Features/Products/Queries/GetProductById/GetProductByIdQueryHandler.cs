using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IDiscountCalculationService _discountCalculationService;

    public GetProductByIdQueryHandler(
        IProductQueryRepository productRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.Id);
        }

        var productDto = product.Adapt<ProductDto>();

        // Endirim hesablaması
        var discountPercent = await _discountCalculationService.CalculateFinalDiscountPercentAsync(
            product.Id,
            product.CategoryId,
            product.BrandId,
            cancellationToken);

        var finalPrice = _discountCalculationService.CalculateDiscountedPrice(
            product.Price.Amount,
            discountPercent);

        // Endirim məlumatlarını DTO-ya əlavə et
        productDto = productDto with
        {
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice
        };

        return Result.Success(productDto);
    }
}





