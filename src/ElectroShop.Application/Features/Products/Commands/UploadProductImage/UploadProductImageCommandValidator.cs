using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.UploadProductImage;

/// <summary>
/// Validator for UploadProductImageCommand
/// Automatically invoked by ValidationBehaviour
/// </summary>
public class UploadProductImageCommandValidator : AbstractValidator<UploadProductImageCommand>
{
    private readonly IQueryRepository<Product> _productRepository;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private static readonly string[] AllowedContentTypes = 
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/gif"
    };
    private const int MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public UploadProductImageCommandValidator(IQueryRepository<Product> productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz")
            .MustAsync(ProductExists)
            .WithMessage("Məhsul tapılmadı");

        // Note: Stream validation is done in FileUploadHelper and controller
        // Command only contains metadata to avoid Swagger schema issues

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("Fayl adı boş ola bilməz")
            .Must(HaveValidExtension)
            .WithMessage($"İcazə verilən fayl formatları: {string.Join(", ", AllowedExtensions)}");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type boş ola bilməz")
            .Must(contentType => AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"İcazə verilən content type-lar: {string.Join(", ", AllowedContentTypes)}");
    }

    private async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
    {
        return await _productRepository.AnyAsync(p => p.Id == productId, cancellationToken);
    }

    private static bool HaveValidExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}

