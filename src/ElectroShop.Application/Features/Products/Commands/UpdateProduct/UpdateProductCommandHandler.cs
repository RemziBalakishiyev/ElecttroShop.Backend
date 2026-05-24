using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Models;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Exceptions;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// UpdateProductCommandHandler - DDD və EF Core Optimistic Concurrency best practices
/// </summary>
public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Brand> _brandRepository;
    private readonly IProductAttributeSchemaResolver _schemaResolver;
    private readonly IProductVariantAttributeValidator _variantValidator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductQueryRepository productQueryRepository,
        IQueryRepository<Category> categoryRepository,
        IQueryRepository<Brand> brandRepository,
        IProductAttributeSchemaResolver schemaResolver,
        IProductVariantAttributeValidator variantValidator,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _schemaResolver = schemaResolver;
        _variantValidator = variantValidator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository
            .GetProductWithImagesAndVariantsAsync(request.Id, cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound(request.Id);

        await _productQueryRepository.EnsureProductImagesAttachedAsync(product, cancellationToken);

        var oldCategoryId = product.CategoryId;

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

        if (category is null || brand is null)
        {
            return Result.Failure<ProductDto>(
                Error.Validation("Product.InvalidRelation", "Category or Brand not found"));
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            product.UpdateDetails(
                name: request.Name,
                description: request.Description,
                price: request.Price,
                currency: request.Currency,
                categoryId: request.CategoryId,
                brandId: request.BrandId,
                vatRate: request.VatRate,
                stock: request.Stock);

            product.SyncImages(request.ImageIds);

            if (request.Variants.Count > 0)
            {
                var variantMaps = request.Variants
                    .Select(v => v.Attributes)
                    .ToList();

                var schemaResult = await _schemaResolver.ResolveAsync(
                    request.CategoryId,
                    request.InlineAttributes,
                    variantMaps,
                    cancellationToken);

                if (schemaResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<ProductDto>(schemaResult.Error);
                }

                var existingVariantAttributes = product.ProductVariants
                    .ToDictionary(v => v.Id, v => v.AttributesJson);

                CategoryChangeContext? categoryChange = oldCategoryId != request.CategoryId
                    ? new CategoryChangeContext(oldCategoryId, request.CategoryId, existingVariantAttributes)
                    : null;

                var normalizedResult = _variantValidator.ValidateAndNormalize(
                    schemaResult.Value,
                    request.Variants,
                    categoryChange);

                if (normalizedResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<ProductDto>(normalizedResult.Error);
                }

                var variantData = normalizedResult.Value
                    .Select(v => (v.Id, v.AttributesJson, v.ImageId, v.IsActive))
                    .ToList();

                try
                {
                    product.SyncVariants(variantData);
                }
                catch (ArgumentException ex)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<ProductDto>(
                        Error.Validation("ProductVariant.InvalidData", ex.Message));
                }
                catch (InvalidOperationException ex)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<ProductDto>(
                        Error.NotFound("ProductVariant.NotFound", ex.Message));
                }

                var activeVariantIds = request.Variants
                    .Where(v => v.Id.HasValue)
                    .Select(v => v.Id!.Value)
                    .ToList();

                product.DeactivateMissingVariants(activeVariantIds);
            }
            else if (oldCategoryId != request.CategoryId && product.ProductVariants.Any(v => v.IsActive))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<ProductDto>(
                    DomainErrors.ProductVariant.CategoryChangeIncompatible);
            }

            await _unitOfWork.PrepareProductAggregateForSaveAsync(product.Id, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure<ProductDto>(
                Error.Conflict(
                    "Product.ConcurrencyConflict",
                    "The data has been modified by another user. Please reload and try again."));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            Sku = product.Sku.Value,
            CategoryId = product.CategoryId,
            CategoryName = category.Name,
            BrandId = product.BrandId,
            BrandName = brand.Name,
            VatRate = product.VatRate,
            Stock = product.Stock,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAtUtc,
            UpdatedAt = product.UpdatedAtUtc,
            RowVersion = product.RowVersion
        };

        return Result.Success(dto);
    }
}
