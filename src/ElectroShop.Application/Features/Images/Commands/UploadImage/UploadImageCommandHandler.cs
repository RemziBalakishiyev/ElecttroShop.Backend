using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ElectroShop.Application.Features.Images.Commands.UploadImage;

/// <summary>
/// Handler for UploadImageCommand
/// Uploads image to Cloudinary and returns imageId
/// </summary>
public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Result<Guid>>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IImageUploadContext _imageUploadContext;

    public UploadImageCommandHandler(
        IImageStorageService imageStorageService,
        IImageUploadContext imageUploadContext)
    {
        _imageStorageService = imageStorageService;
        _imageUploadContext = imageUploadContext;
    }

    public async Task<Result<Guid>> Handle(
        UploadImageCommand request,
        CancellationToken cancellationToken)
    {
        if (_imageUploadContext.ImageStream == null)
        {
            return Result.Failure<Guid>(
                Error.Validation("ImageStream.Required", "Şəkil stream-i tələb olunur"));
        }

        var formFile = CreateFormFile(
            _imageUploadContext.ImageStream,
            request.FileName,
            request.ContentType);

        try
        {
            var uploadResult = await _imageStorageService.UploadAsync(formFile, cancellationToken: cancellationToken);
            var imageId = ExtractImageId(uploadResult.PublicId);
            return Result.Success(imageId);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(Error.Validation("Image.Invalid", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<Guid>(Error.Failure("Image.UploadFailed", ex.Message));
        }
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
