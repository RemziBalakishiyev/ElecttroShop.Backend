using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.UploadProductImage;

/// <summary>
/// UploadProductImageCommand üçün Handler
/// DDD və Clean Architecture prinsiplərinə uyğundur
/// </summary>
public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, Result<ProductDto>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IImageStorage _imageStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageUploadContext _imageUploadContext;

    public UploadProductImageCommandHandler(
        IProductQueryRepository productQueryRepository,
        IImageStorage imageStorage,
        IUnitOfWork unitOfWork,
        IImageUploadContext imageUploadContext)
    {
        _productQueryRepository = productQueryRepository;
        _imageStorage = imageStorage;
        _unitOfWork = unitOfWork;
        _imageUploadContext = imageUploadContext;
    }

    public async Task<Result<ProductDto>> Handle(
        UploadProductImageCommand request,
        CancellationToken cancellationToken)
    {
        if (_imageUploadContext.ImageStream == null)
        {
            return Result.Failure<ProductDto>(
                Error.Validation("ImageStream.Required", "Şəkil stream-i tələb olunur"));
        }

        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound(request.ProductId);

        await _productQueryRepository.EnsureProductImagesAttachedAsync(product, cancellationToken);

        foreach (var image in product.ProductImages.Where(pi => pi.IsPrimary))
            image.RemoveAsPrimary();

        var imageId = await _imageStorage.UploadImageAsync(
            _imageUploadContext.ImageStream,
            request.FileName,
            request.ContentType,
            cancellationToken);

        var displayOrder = product.ProductImages.Any()
            ? product.ProductImages.Max(pi => pi.DisplayOrder) + 1
            : 0;

        product.AddImage(imageId, displayOrder, isPrimary: true);

        await _unitOfWork.PrepareProductAggregateForSaveAsync(product.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProduct = await _productQueryRepository.GetProductWithDetailsAsync(
            request.ProductId,
            cancellationToken);

        if (updatedProduct is null)
            return DomainErrors.Product.NotFound(request.ProductId);

        return Result.Success(updatedProduct.Adapt<ProductDto>());
    }
}
