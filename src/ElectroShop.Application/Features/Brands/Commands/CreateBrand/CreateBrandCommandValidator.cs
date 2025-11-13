using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    private readonly IQueryRepository<Brand> _brandRepository;

    public CreateBrandCommandValidator(IQueryRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Brend adı boş ola bilməz")
            .MaximumLength(200)
            .WithMessage("Brend adı maksimum 200 simvol ola bilər")
            .MinimumLength(2)
            .WithMessage("Brend adı minimum 2 simvol olmalıdır")
            .MustAsync(BeUniqueName)
            .WithMessage("Bu adda brend artıq mövcuddur");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        var existing = await _brandRepository.FirstOrDefaultAsync(
            b => b.Name.ToLower() == name.ToLower(),
            cancellationToken);
        return existing is null;
    }
}

