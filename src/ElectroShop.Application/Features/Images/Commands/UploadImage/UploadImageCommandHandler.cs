using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Services;
using MediatR;

namespace ElectroShop.Application.Features.Images.Commands.UploadImage;

/// <summary>
/// Handler for UploadImageCommand
/// Uploads image and returns imageId
/// </summary>
public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Result<Guid>>
{
    private readonly IImageStorage _imageStorage;
    private readonly IImageUploadContext _imageUploadContext;

    public UploadImageCommandHandler(
        IImageStorage imageStorage,
        IImageUploadContext imageUploadContext)
    {
        _imageStorage = imageStorage;
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

        var imageId = await _imageStorage.UploadImageAsync(
            _imageUploadContext.ImageStream,
            request.FileName,
            request.ContentType,
            cancellationToken);

        return Result.Success(imageId);
    }
}

