using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Exceptions;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// CreateProductCommand üçün Handler
/// Bütün validation FluentValidation-da, domain logic Domain layer-də
/// DDD və Clean Architecture prinsiplərinə uyğundur
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IWriteRepository<Product> _productRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Brand> _brandRepository;
    private readonly IProductAttributeSchemaResolver _schemaResolver;
    private readonly IProductVariantAttributeValidator _variantValidator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IWriteRepository<Product> productRepository,
        IQueryRepository<Category> categoryRepository,
        IQueryRepository<Brand> brandRepository,
        IProductAttributeSchemaResolver schemaResolver,
        IProductVariantAttributeValidator variantValidator,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _schemaResolver = schemaResolver;
        _variantValidator = variantValidator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
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
            List<(Guid? Id, string AttributesJson, Guid? ImageId, bool IsActive)>? variantData = null;

            var hasVariants = request.Variants.Count > 0;

            if (hasVariants)
            {
                var variantMaps = request.Variants.Select(v => v.Attributes).ToList();

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

                var normalizedResult = _variantValidator.ValidateAndNormalize(
                    schemaResult.Value,
                    request.Variants,
                    categoryChange: null);

                if (normalizedResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<ProductDto>(normalizedResult.Error);
                }

                variantData = normalizedResult.Value
                    .Select(v => (v.Id, v.AttributesJson, v.ImageId, v.IsActive))
                    .ToList();
            }

            Product product;
            try
            {
                product = Product.Create(
                    name: request.Name,
                    sku: request.Sku,
                    categoryId: request.CategoryId,
                    brandId: request.BrandId,
                    price: request.Price,
                    currency: request.Currency,
                    vatRate: request.VatRate,
                    stock: request.Stock,
                    description: request.Description);
            }
            catch (ArgumentException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<ProductDto>(
                    Error.Validation("Product.InvalidData", ex.Message));
            }

            if (request.ImageIds.Count > 0)
            {
                for (var i = 0; i < request.ImageIds.Count; i++)
                {
                    product.AddImage(request.ImageIds[i], i, isPrimary: i == 0);
                }
            }

            product.SyncAttributes(ProductAttributeDraftMapper.ToDrafts(request.InlineAttributes));

            if (variantData is not null)
            {
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
            }

            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.PrepareProductAggregateForSaveAsync(product.Id, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var productDto = new ProductDto
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
                UpdatedAt = product.UpdatedAtUtc
            };

            return Result.Success(productDto);
        }
        catch (ConcurrencyException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure<ProductDto>(
                Error.Conflict("Product.ConcurrencyConflict", ex.Message));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
