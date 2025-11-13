using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly IQueryRepository<Category> _categoryRepository;

    public CreateCategoryCommandValidator(IQueryRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Kateqoriya adı boş ola bilməz")
            .MaximumLength(200)
            .WithMessage("Kateqoriya adı maksimum 200 simvol ola bilər")
            .MinimumLength(2)
            .WithMessage("Kateqoriya adı minimum 2 simvol olmalıdır")
            .MustAsync(BeUniqueName)
            .WithMessage("Bu adda kateqoriya artıq mövcuddur");

        RuleFor(x => x.Slug)
            .MaximumLength(200)
            .WithMessage("Slug maksimum 200 simvol ola bilər")
            .Matches(@"^[a-z0-9\-]*$")
            .WithMessage("Slug yalnız kiçik hərf, rəqəm və tire simvollarından ibarət ola bilər")
            .When(x => !string.IsNullOrEmpty(x.Slug));

        When(x => x.ParentId.HasValue, () =>
        {
            RuleFor(x => x.ParentId!.Value)
                .MustAsync(ParentExists)
                .WithMessage("Parent kateqoriya tapılmadı");
        });
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        var existing = await _categoryRepository.FirstOrDefaultAsync(
            c => c.Name.ToLower() == name.ToLower(),
            cancellationToken);
        return existing is null;
    }

    private async Task<bool> ParentExists(Guid parentId, CancellationToken cancellationToken)
    {
        return await _categoryRepository.AnyAsync(c => c.Id == parentId, cancellationToken);
    }
}

