using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Images.Commands.UploadImage;

/// <summary>
/// Command to upload image without product (standalone)
/// Returns imageId for use in product creation
/// </summary>
public record UploadImageCommand : IRequest<Result<Guid>>
{
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}

