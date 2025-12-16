using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;
using MediatR;
using System.Text.Json;

namespace ElectroShop.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// CreateProductCommand üçün Handler
/// Bütün validation FluentValidation-da, domain logic Domain layer-də
/// DDD və Clean Architecture prinsiplərinə uyğundur
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IWriteRepository<Product> _productRepository;
    private readonly IWriteRepository<ProductVariant> _variantRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IWriteRepository<Product> productRepository,
        IWriteRepository<ProductVariant> variantRepository,
        IQueryRepository<Category> categoryRepository,
        IQueryRepository<Brand> brandRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

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
                description: request.Description
            );
        }
        catch (ArgumentException ex)
        {
            // Value object yaradılması uğursuz (validasiya düzgündürsə baş verməməlidir)
            return Result.Failure<ProductDto>(
                Error.Validation("Product.InvalidData", ex.Message));
        }

        // Şəkilləri əlavə et
        if (request.ImageIds.Any())
        {
            for (int i = 0; i < request.ImageIds.Count; i++)
            {
                var isPrimary = i == 0; // İlk şəkil əsas şəkil olur
                product.AddImage(request.ImageIds[i], i, isPrimary);
            }
        }

        // Yadda saxla
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Variantları əlavə et (product yaradıldıqdan sonra)
        foreach (var variantDto in request.Variants)
        {
            var attributesJson = JsonSerializer.Serialize(variantDto.Attributes);
            var variant = ProductVariant.Create(
                product.Id,
                attributesJson,
                variantDto.ImageId
            );
            await _variantRepository.AddAsync(variant, cancellationToken);
        }

        if (request.Variants.Any())
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            Sku = product.Sku.Value,
            CategoryId = product.CategoryId,
            CategoryName = category!.Name,
            BrandId = product.BrandId,
            BrandName = brand!.Name,
            VatRate = product.VatRate,
            Stock = product.Stock,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAtUtc,
            UpdatedAt = product.UpdatedAtUtc
        };

        return Result.Success(productDto);
    }
}


