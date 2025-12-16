using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Features.Images.Commands.UploadImage;
using ElectroShop.Application.Features.Products.Commands.UploadProductImage;
using ElectroShop.Application.Services;
using Microsoft.AspNetCore.Http;
using Error = ElectroShop.Application.Common.Results.Error;

namespace ElectroShop.WebApi.Helpers;

/// <summary>
/// File yükləmə əməliyyatları üçün helper
/// </summary>
public static class FileUploadHelper
{
    /// <summary>
    /// IFormFile-dan UploadProductImageCommand yaradır və stream-i context-ə yazır
    /// </summary>
    public static async Task<Result<UploadProductImageCommand>> CreateUploadProductImageCommandAsync(
        IFormFile file,
        Guid productId,
        IImageUploadContext imageUploadContext,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return Result.Failure<UploadProductImageCommand>(
                Error.Validation("Image.Required", "Şəkil faylı tələb olunur"));
        }

        var memoryStream = await ConvertToMemoryStreamAsync(file, cancellationToken);
        
        // Stream-i context-ə yazırıq (handler buradan alacaq)
        imageUploadContext.ImageStream = memoryStream;

        var command = new UploadProductImageCommand
        {
            ProductId = productId,
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        return Result.Success(command);
    }

    /// <summary>
    /// IFormFile-dan UploadImageCommand yaradır və stream-i context-ə yazır (standalone)
    /// </summary>
    public static async Task<Result<UploadImageCommand>> CreateUploadImageCommandAsync(
        IFormFile file,
        IImageUploadContext imageUploadContext,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return Result.Failure<UploadImageCommand>(
                Error.Validation("Image.Required", "Şəkil faylı tələb olunur"));
        }

        var memoryStream = await ConvertToMemoryStreamAsync(file, cancellationToken);
        
        // Stream-i context-ə yazırıq (handler buradan alacaq)
        imageUploadContext.ImageStream = memoryStream;

        var command = new UploadImageCommand
        {
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        return Result.Success(command);
    }

    private static async Task<MemoryStream> ConvertToMemoryStreamAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();
        await using var stream = file.OpenReadStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        return memoryStream;
    }
}

