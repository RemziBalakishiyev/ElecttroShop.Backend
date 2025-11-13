using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.ValueObjects;
using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// UpdateProductCommand üçün Validator
/// Async validation ilə business rules
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly IQueryRepository<Product> _productRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Brand> _brandRepository;

    public UpdateProductCommandValidator(
        IQueryRepository<Product> productRepository,
        IQueryRepository<Category> categoryRepository,
        IQueryRepository<Brand> brandRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz")
            .MustAsync(ProductExists)
            .WithMessage("Məhsul tapılmadı");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Məhsulun adı boş ola bilməz")
            .MaximumLength(200)
            .WithMessage("Məhsulun adı maksimum 200 simvol ola bilər")
            .MinimumLength(3)
            .WithMessage("Məhsulun adı minimum 3 simvol olmalıdır");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Məhsulun təsviri maksimum 2000 simvol ola bilər")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Qiymət 0-dan böyük olmalıdır")
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("Qiymət 1,000,000-dan kiçik və ya bərabər olmalıdır");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Valyuta boş ola bilməz")
            .Length(3)
            .WithMessage("Valyuta 3 simvol olmalıdır (məs: TRY, USD, EUR)")
            .Must(currency => Money.ValidCurrencies.Contains(currency.ToUpperInvariant()))
            .WithMessage($"Yanlış valyuta. Etibarlı valyutalar: {string.Join(", ", Money.ValidCurrencies)}");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Kateqoriya seçilməlidir")
            .MustAsync(CategoryExists)
            .WithMessage("Seçilmiş kateqoriya tapılmadı");

        RuleFor(x => x.BrandId)
            .NotEmpty()
            .WithMessage("Brend seçilməlidir")
            .MustAsync(BrandExists)
            .WithMessage("Seçilmiş brend tapılmadı");

        RuleFor(x => x.VatRate)
            .InclusiveBetween(0, 1)
            .WithMessage("ƏDV dərəcəsi 0 ilə 1 arasında olmalıdır (məs: 0.18)");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stok miqdarı mənfi ola bilməz");
    }

    private async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
    {
        return await _productRepository.AnyAsync(p => p.Id == productId, cancellationToken);
    }

    private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _categoryRepository.AnyAsync(c => c.Id == categoryId, cancellationToken);
    }

    private async Task<bool> BrandExists(Guid brandId, CancellationToken cancellationToken)
    {
        return await _brandRepository.AnyAsync(b => b.Id == brandId, cancellationToken);
    }
}

