using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ElectroShop.Application.Features.Images.Commands.BackfillCloudinaryImages;

public class BackfillCloudinaryImagesCommandHandler
    : IRequestHandler<BackfillCloudinaryImagesCommand, Result<BackfillCloudinaryImagesResultDto>>
{
    private readonly IProductImageQueryRepository _productImageQueryRepository;
    private readonly IImageStorage _imageStorage;
    private readonly IImageStorageService _imageStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BackfillCloudinaryImagesCommandHandler> _logger;

    public BackfillCloudinaryImagesCommandHandler(
        IProductImageQueryRepository productImageQueryRepository,
        IImageStorage imageStorage,
        IImageStorageService imageStorageService,
        IUnitOfWork unitOfWork,
        ILogger<BackfillCloudinaryImagesCommandHandler> logger)
    {
        _productImageQueryRepository = productImageQueryRepository;
        _imageStorage = imageStorage;
        _imageStorageService = imageStorageService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BackfillCloudinaryImagesResultDto>> Handle(
        BackfillCloudinaryImagesCommand request,
        CancellationToken cancellationToken)
    {
        var candidates = await _productImageQueryRepository
            .GetImagesNeedingCloudinaryBackfillAsync(cancellationToken);

        var migrated = new List<BackfillCloudinaryImageItemDto>();
        var skipped = new List<BackfillCloudinaryImageItemDto>();
        var failed = new List<BackfillCloudinaryImageItemDto>();

        foreach (var productImage in candidates)
        {
            var imageData = await _imageStorage.GetImageAsync(productImage.ImageId, cancellationToken);
            if (imageData == null)
            {
                _logger.LogWarning(
                    "Cloudinary backfill skipped: local file not found. ProductImageId={ProductImageId}, ImageId={ImageId}, ProductId={ProductId}",
                    productImage.Id,
                    productImage.ImageId,
                    productImage.ProductId);

                skipped.Add(new BackfillCloudinaryImageItemDto
                {
                    ProductImageId = productImage.Id,
                    ProductId = productImage.ProductId,
                    ImageId = productImage.ImageId,
                    Reason = "Local disk file not found"
                });
                continue;
            }

            await using var stream = imageData.Value.Stream;
            var extension = await _imageStorage.GetImageExtensionAsync(productImage.ImageId, cancellationToken) ?? ".jpg";
            var contentType = !string.IsNullOrWhiteSpace(productImage.ContentType)
                ? productImage.ContentType
                : imageData.Value.ContentType;
            var fileName = !string.IsNullOrWhiteSpace(productImage.FileName)
                ? productImage.FileName
                : $"{productImage.ImageId}{extension}";

            var formFile = CreateFormFile(stream, fileName, contentType);

            try
            {
                var uploadResult = await _imageStorageService.UploadAsync(
                    formFile,
                    imageId: productImage.ImageId,
                    cancellationToken: cancellationToken);

                var imageUrl = uploadResult.SecureUrl ?? uploadResult.Url;
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    throw new InvalidOperationException("Cloudinary upload returned no URL.");
                }

                productImage.SetStorageMetadata(
                    imageUrl,
                    uploadResult.PublicId,
                    uploadResult.FileName,
                    uploadResult.ContentType,
                    uploadResult.Size,
                    "Cloudinary");

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Cloudinary backfill migrated. ProductImageId={ProductImageId}, ImageId={ImageId}, PublicId={PublicId}",
                    productImage.Id,
                    productImage.ImageId,
                    uploadResult.PublicId);

                migrated.Add(new BackfillCloudinaryImageItemDto
                {
                    ProductImageId = productImage.Id,
                    ProductId = productImage.ProductId,
                    ImageId = productImage.ImageId,
                    ImageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Cloudinary backfill failed. ProductImageId={ProductImageId}, ImageId={ImageId}, ProductId={ProductId}",
                    productImage.Id,
                    productImage.ImageId,
                    productImage.ProductId);

                failed.Add(new BackfillCloudinaryImageItemDto
                {
                    ProductImageId = productImage.Id,
                    ProductId = productImage.ProductId,
                    ImageId = productImage.ImageId,
                    Reason = ex.Message
                });
            }
        }

        return Result.Success(new BackfillCloudinaryImagesResultDto
        {
            TotalCandidates = candidates.Count,
            MigratedCount = migrated.Count,
            SkippedCount = skipped.Count,
            FailedCount = failed.Count,
            Migrated = migrated,
            Skipped = skipped,
            Failed = failed
        });
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
}
