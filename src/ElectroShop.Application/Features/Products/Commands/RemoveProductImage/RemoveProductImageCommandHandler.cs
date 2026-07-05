using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using ElectroShop.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductImage;

public class RemoveProductImageCommandHandler 
    : IRequestHandler<RemoveProductImageCommand, Result>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorageService;
    private readonly IImageStorage _imageStorage;
    private readonly ILogger<RemoveProductImageCommandHandler> _logger;

    public RemoveProductImageCommandHandler(
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork,
        IImageStorageService imageStorageService,
        IImageStorage imageStorage,
        ILogger<RemoveProductImageCommandHandler> logger)
    {
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;
        _imageStorage = imageStorage;
        _logger = logger;
    }

    public async Task<Result> Handle(
        RemoveProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.ProductId, 
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        await _productQueryRepository.EnsureProductImagesAttachedAsync(product, cancellationToken);

        var imageToRemove = product.ProductImages
            .FirstOrDefault(pi => pi.ImageId == request.ImageId);

        if (imageToRemove is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        await DeleteStoredImageAsync(imageToRemove, cancellationToken);

        product.RemoveImage(request.ImageId);

        await _unitOfWork.PrepareProductAggregateForSaveAsync(product.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task DeleteStoredImageAsync(ProductImage image, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(image.PublicId))
        {
            try
            {
                await _imageStorageService.DeleteAsync(image.PublicId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Cloudinary delete failed for PublicId: {PublicId}, ImageId: {ImageId}",
                    image.PublicId,
                    image.ImageId);
            }

            return;
        }

        try
        {
            await _imageStorage.DeleteImageAsync(image.ImageId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Local image delete failed for ImageId: {ImageId}",
                image.ImageId);
        }
    }
}
