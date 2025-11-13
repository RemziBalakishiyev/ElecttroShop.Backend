using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
{
    private readonly IQueryRepository<Brand> _brandRepository;

    public DeleteBrandCommandValidator(IQueryRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Brend ID-si boş ola bilməz")
            .MustAsync(BrandExists)
            .WithMessage("Brend tapılmadı");
    }

    private async Task<bool> BrandExists(Guid id, CancellationToken cancellationToken)
    {
        return await _brandRepository.AnyAsync(b => b.Id == id, cancellationToken);
    }
}

