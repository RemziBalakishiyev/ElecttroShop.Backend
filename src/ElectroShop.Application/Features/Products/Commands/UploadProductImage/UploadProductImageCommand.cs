using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.UploadProductImage;

/// <summary>
/// Command to upload product image
/// Returns Result pattern with ProductDto
/// Note: Stream is passed separately to handler to avoid Swagger schema issues
/// </summary>
public record UploadProductImageCommand : IRequest<Result<ProductDto>>
{
    public Guid ProductId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}


