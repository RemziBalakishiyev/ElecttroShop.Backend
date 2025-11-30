using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Brands.Commands.UpdateBrand;

public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
{
    private readonly IQueryRepository<Brand> _brandRepository;

    public UpdateBrandCommandValidator(IQueryRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Brend ID-si boş ola bilməz")
            .MustAsync(BrandExists)
            .WithMessage("Brend tapılmadı");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Brend adı boş ola bilməz")
            .MaximumLength(200)
            .WithMessage("Brend adı maksimum 200 simvol ola bilər")
            .MinimumLength(2)
            .WithMessage("Brend adı minimum 2 simvol olmalıdır");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DisplayOrder.HasValue)
            .WithMessage("DisplayOrder 0 və ya daha böyük olmalıdır");
    }

    private async Task<bool> BrandExists(Guid id, CancellationToken cancellationToken)
    {
        return await _brandRepository.AnyAsync(b => b.Id == id, cancellationToken);
    }
}

