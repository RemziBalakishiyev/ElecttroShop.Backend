using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.ValueObjects;
using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Validator for CreateProductCommand
/// Automatically invoked by ValidationBehaviour
/// Includes async validation for business rules
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IQueryRepository<Product> _productRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Brand> _brandRepository;

    public CreateProductCommandValidator(
        IQueryRepository<Product> productRepository,
        IQueryRepository<Category> categoryRepository,
        IQueryRepository<Brand> brandRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;

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

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU boş ola bilməz")
            .MaximumLength(Sku.MaxLength)
            .WithMessage($"SKU maksimum {Sku.MaxLength} simvol ola bilər")
            .MinimumLength(Sku.MinLength)
            .WithMessage($"SKU minimum {Sku.MinLength} simvol olmalıdır")
            .Matches(Sku.SkuPattern)
            .WithMessage("SKU yalnız böyük hərflər, rəqəmlər, tire və alt xətt simvollarından ibarət ola bilər")
            .MustAsync(BeUniqueSku)
            .WithMessage("Bu SKU artıq istifadə olunur");

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

        RuleForEach(x => x.InlineAttributes)
            .ChildRules(inline =>
            {
                inline.RuleFor(a => a.Name)
                    .NotEmpty()
                    .WithMessage("Attribute adı boş ola bilməz");

                inline.RuleFor(a => a.AttributeType)
                    .NotEmpty()
                    .WithMessage("AttributeType boş ola bilməz");

                inline.RuleForEach(a => a.Values)
                    .ChildRules(value =>
                    {
                        value.RuleFor(v => v.Value)
                            .NotEmpty()
                            .WithMessage("Attribute dəyəri boş ola bilməz")
                            .MaximumLength(CategoryAttributeValue.MaxValueLength)
                            .WithMessage($"Attribute dəyəri maksimum {CategoryAttributeValue.MaxValueLength} simvol ola bilər");
                    });
            })
            .When(x => x.InlineAttributes is not null);

        RuleFor(x => x.InlineAttributes)
            .Must(NotHaveDuplicateAttributeTypes!)
            .WithMessage("Inline attribute siyahısında eyni AttributeType təkrarlanır")
            .When(x => x.InlineAttributes is not null);

        RuleForEach(x => x.InlineAttributes!)
            .Must(inline => NotHaveDuplicateValues(inline.Values))
            .WithMessage("Inline attribute dəyərləri təkrarlanır")
            .When(x => x.InlineAttributes is not null);

        RuleForEach(x => x.Variants)
            .Must(v => v.Attributes is not null && v.Attributes.Count > 0)
            .WithMessage("Variant attribute-ları boş ola bilməz")
            .When(x => x.Variants.Count > 0);
    }

    private static bool NotHaveDuplicateAttributeTypes(IReadOnlyList<InlineProductAttributeDto> inlineAttributes)
    {
        if (inlineAttributes.Count == 0)
            return true;

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var inline in inlineAttributes)
        {
            var key = AttributeTypeNormalizer.Normalize(inline.AttributeType);
            if (string.IsNullOrEmpty(key))
                continue;

            if (!seen.Add(key))
                return false;
        }

        return true;
    }

    private static bool NotHaveDuplicateValues(IReadOnlyList<InlineProductAttributeValueDto> values)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in values)
        {
            var normalized = AttributeTypeNormalizer.NormalizeValue(value.Value);
            if (string.IsNullOrEmpty(normalized))
                continue;

            if (!seen.Add(normalized))
                return false;
        }

        return true;
    }

    private async Task<bool> BeUniqueSku(string sku, CancellationToken cancellationToken)
    {
        var normalizedSku = sku.Trim().ToUpperInvariant();
        var existingProduct = await _productRepository.FirstOrDefaultAsync(
            p => p.Sku.Value == normalizedSku,
            cancellationToken);

        return existingProduct is null;
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
