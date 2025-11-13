using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// DeleteProductCommand üçün Validator
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    private readonly IQueryRepository<Product> _productRepository;

    public DeleteProductCommandValidator(IQueryRepository<Product> productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz")
            .MustAsync(ProductExists)
            .WithMessage("Məhsul tapılmadı");
    }

    private async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
    {
        return await _productRepository.AnyAsync(p => p.Id == productId, cancellationToken);
    }
}

