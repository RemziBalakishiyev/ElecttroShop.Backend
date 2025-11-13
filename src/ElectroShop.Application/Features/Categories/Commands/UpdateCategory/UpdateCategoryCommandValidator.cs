using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly IQueryRepository<Category> _categoryRepository;

    public UpdateCategoryCommandValidator(IQueryRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Kateqoriya ID-si boş ola bilməz")
            .MustAsync(CategoryExists)
            .WithMessage("Kateqoriya tapılmadı");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Kateqoriya adı boş ola bilməz")
            .MaximumLength(200)
            .WithMessage("Kateqoriya adı maksimum 200 simvol ola bilər")
            .MinimumLength(2)
            .WithMessage("Kateqoriya adı minimum 2 simvol olmalıdır");

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
                .WithMessage("Parent kateqoriya tapılmadı")
                .MustAsync((command, parentId, ct) => NotSelfParent(command.Id, parentId, ct))
                .WithMessage("Kateqoriya özünün parent-i ola bilməz");
        });
    }

    private async Task<bool> CategoryExists(Guid id, CancellationToken cancellationToken)
    {
        return await _categoryRepository.AnyAsync(c => c.Id == id, cancellationToken);
    }

    private async Task<bool> ParentExists(Guid parentId, CancellationToken cancellationToken)
    {
        return await _categoryRepository.AnyAsync(c => c.Id == parentId, cancellationToken);
    }

    private Task<bool> NotSelfParent(Guid categoryId, Guid parentId, CancellationToken cancellationToken)
    {
        return Task.FromResult(categoryId != parentId);
    }
}

