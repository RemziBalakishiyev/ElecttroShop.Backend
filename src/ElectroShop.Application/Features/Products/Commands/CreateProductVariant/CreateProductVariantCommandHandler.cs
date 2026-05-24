using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Exceptions;
using MediatR;
using System.Text.Json;

namespace ElectroShop.Application.Features.Products.Commands.CreateProductVariant;

/// <summary>
/// CreateProductVariantCommandHandler - DDD Aggregate pattern
/// Variant yalnız Product aggregate vasitəsilə əlavə edilir
/// </summary>
public class CreateProductVariantCommandHandler 
    : IRequestHandler<CreateProductVariantCommand, Result<ProductVariantDto>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IDiscountCalculationService _discountCalculationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductVariantCommandHandler(
        IProductQueryRepository productQueryRepository,
        IDiscountCalculationService discountCalculationService,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _discountCalculationService = discountCalculationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductVariantDto>> Handle(
        CreateProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        // Product aggregate load (tracked)
        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.ProductId, 
            cancellationToken);
        
        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.ProductId);
        }

        // Aggregate metod vasitəsilə variant əlavə et
        var attributesJson = JsonSerializer.Serialize(request.Attributes);
        var variant = product.AddVariant(attributesJson, request.ImageId);

        // Tracked entity üçün Update() çağırmaq QADAĞANDIR
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException ex)
        {
            return Result.Failure<ProductVariantDto>(
                Error.Conflict(
                    "Product.ConcurrencyConflict",
                    ex.Message
                ));
        }

        // Endirim hesabla (Product-dan)
        var discountPercent = await _discountCalculationService.CalculateFinalDiscountPercentAsync(
            product.Id,
            product.CategoryId,
            product.BrandId,
            cancellationToken);

        var finalPrice = _discountCalculationService.CalculateDiscountedPrice(
            product.Price.Amount,
            discountPercent);

        var variantDto = new ProductVariantDto
        {
            Id = variant.Id,
            Sku = product.Sku.Value, // Product-dan
            Price = product.Price.Amount, // Product-dan
            Currency = product.Price.Currency, // Product-dan
            Stock = product.Stock, // Product-dan
            IsActive = variant.IsActive,
            ImageId = variant.ImageId,
            ImageUrl = variant.ImageId.HasValue ? $"/api/images/{variant.ImageId}" : null,
            Attributes = request.Attributes,
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice
        };

        return Result.Success(variantDto);
    }
}



