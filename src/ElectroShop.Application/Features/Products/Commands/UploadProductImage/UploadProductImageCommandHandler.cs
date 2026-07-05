using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ElectroShop.Application.Features.Products.Commands.UploadProductImage;

/// <summary>
/// UploadProductImageCommand üçün Handler
/// DDD və Clean Architecture prinsiplərinə uyğundur
/// </summary>
public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, Result<ProductDto>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageUploadContext _imageUploadContext;
    private readonly IImageUrlResolver _imageUrlResolver;

    public UploadProductImageCommandHandler(
        IProductQueryRepository productQueryRepository,
        IImageStorageService imageStorageService,
        IUnitOfWork unitOfWork,
        IImageUploadContext imageUploadContext,
        IImageUrlResolver imageUrlResolver)
    {
        _productQueryRepository = productQueryRepository;
        _imageStorageService = imageStorageService;
        _unitOfWork = unitOfWork;
        _imageUploadContext = imageUploadContext;
        _imageUrlResolver = imageUrlResolver;
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

        var formFile = CreateFormFile(
            _imageUploadContext.ImageStream,
            request.FileName,
            request.ContentType);

        ImageUploadResultDto uploadResult;
        try
        {
            uploadResult = await _imageStorageService.UploadAsync(formFile, cancellationToken: cancellationToken);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<ProductDto>(Error.Validation("Image.Invalid", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<ProductDto>(Error.Failure("Image.UploadFailed", ex.Message));
        }

        var imageId = ExtractImageId(uploadResult.PublicId);

        var displayOrder = product.ProductImages.Any()
            ? product.ProductImages.Max(pi => pi.DisplayOrder) + 1
            : 0;

        product.AddImage(
            imageId,
            displayOrder,
            isPrimary: true,
            imageUrl: uploadResult.SecureUrl ?? uploadResult.Url,
            publicId: uploadResult.PublicId,
            fileName: uploadResult.FileName,
            contentType: uploadResult.ContentType,
            size: uploadResult.Size,
            storageProvider: uploadResult.StorageProvider);

        await _unitOfWork.PrepareProductAggregateForSaveAsync(product.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProduct = await _productQueryRepository.GetProductWithDetailsAsync(
            request.ProductId,
            cancellationToken);

        if (updatedProduct is null)
            return DomainErrors.Product.NotFound(request.ProductId);

        var productDto = updatedProduct.Adapt<ProductDto>();
        var primaryImage = updatedProduct.ProductImages
            .OrderBy(pi => pi.IsPrimary ? 0 : 1)
            .ThenBy(pi => pi.DisplayOrder)
            .FirstOrDefault();

        productDto = productDto with
        {
            PrimaryImageUrl = primaryImage != null
                ? await _imageUrlResolver.ResolveProductImageUrlAsync(primaryImage, cancellationToken)
                : uploadResult.SecureUrl ?? uploadResult.Url,
            Images = await BuildImageDtosAsync(updatedProduct.ProductImages, cancellationToken)
        };

        return Result.Success(productDto);
    }

    private async Task<List<ProductImageDto>> BuildImageDtosAsync(
        IEnumerable<ProductImage> images,
        CancellationToken cancellationToken)
    {
        var result = new List<ProductImageDto>();

        foreach (var image in images.OrderBy(pi => pi.DisplayOrder))
        {
            result.Add(new ProductImageDto
            {
                Id = image.Id,
                ImageId = image.ImageId,
                ImageUrl = await _imageUrlResolver.ResolveProductImageUrlAsync(image, cancellationToken),
                DisplayOrder = image.DisplayOrder,
                IsPrimary = image.IsPrimary
            });
        }

        return result;
    }

    private static IFormFile CreateFormFile(Stream stream, string fileName, string contentType)
    {
        if (stream.CanSeek)
            stream.Position = 0;

        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static Guid ExtractImageId(string publicId)
    {
        var lastSegment = publicId.Split('/').LastOrDefault();
        return Guid.TryParse(lastSegment, out var imageId)
            ? imageId
            : Guid.NewGuid();
    }
}
