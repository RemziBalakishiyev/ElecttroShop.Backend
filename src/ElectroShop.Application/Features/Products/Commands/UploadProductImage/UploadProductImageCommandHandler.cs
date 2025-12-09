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
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IProductQueryRepository _productQueryRepositoryWithDetails;
    private readonly IImageStorage _imageStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageUploadContext _imageUploadContext;

    public UploadProductImageCommandHandler(
        IQueryRepository<Product> productQueryRepository,
        IWriteRepository<Product> productWriteRepository,
        IProductQueryRepository productQueryRepositoryWithDetails,
        IImageStorage imageStorage,
        IUnitOfWork unitOfWork,
        IImageUploadContext imageUploadContext)
    {
        _productQueryRepository = productQueryRepository;
        _productWriteRepository = productWriteRepository;
        _productQueryRepositoryWithDetails = productQueryRepositoryWithDetails;
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

        var product = await _productQueryRepositoryWithDetails.GetProductWithImagesAndVariantsAsync(
            request.ProductId, 
            cancellationToken);
        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.ProductId);
        }

        // Köhnə primary şəkli sil
        var primaryImage = product.ProductImages.FirstOrDefault(pi => pi.IsPrimary);
        if (primaryImage != null)
        {
            await _imageStorage.DeleteImageAsync(primaryImage.ImageId, cancellationToken);
            product.RemoveImage(primaryImage.ImageId);
        }

        var imageId = await _imageStorage.UploadImageAsync(
            _imageUploadContext.ImageStream,
            request.FileName,
            request.ContentType,
            cancellationToken);

        // Yeni şəkli primary olaraq əlavə et
        var displayOrder = product.ProductImages.Any() 
            ? product.ProductImages.Max(pi => pi.DisplayOrder) + 1 
            : 0;
        product.AddImage(imageId, displayOrder, isPrimary: true);
        
        _productWriteRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProduct = await _productQueryRepositoryWithDetails.GetProductWithDetailsAsync(request.ProductId, cancellationToken);
        if (updatedProduct is null)
        {
            return DomainErrors.Product.NotFound(request.ProductId);
        }

        var productDto = updatedProduct.Adapt<ProductDto>();
        return Result.Success(productDto);
    }
}

