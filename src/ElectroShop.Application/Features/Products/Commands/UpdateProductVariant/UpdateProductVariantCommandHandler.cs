using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProductVariant;

public class UpdateProductVariantCommandHandler 
    : IRequestHandler<UpdateProductVariantCommand, Result<ProductVariantDto>>
{
    private readonly IWriteRepository<ProductVariant> _variantRepository;
    private readonly IQueryRepository<ProductVariant> _variantQueryRepository;
    private readonly IQueryRepository<Product> _productRepository;
    private readonly IDiscountCalculationService _discountCalculationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductVariantCommandHandler(
        IWriteRepository<ProductVariant> variantRepository,
        IQueryRepository<ProductVariant> variantQueryRepository,
        IQueryRepository<Product> productRepository,
        IDiscountCalculationService discountCalculationService,
        IUnitOfWork unitOfWork)
    {
        _variantRepository = variantRepository;
        _variantQueryRepository = variantQueryRepository;
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductVariantDto>> Handle(
        UpdateProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        var variant = await _variantQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (variant is null)
        {
            return DomainErrors.Product.NotFound(request.Id);
        }

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.ProductId);
        }

        var attributesJson = JsonSerializer.Serialize(request.Attributes);
        variant.Update(
            attributesJson,
            request.ImageId
        );

        if (request.IsActive && !variant.IsActive)
        {
            variant.Activate();
        }
        else if (!request.IsActive && variant.IsActive)
        {
            variant.Deactivate();
        }

        _variantRepository.Update(variant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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



