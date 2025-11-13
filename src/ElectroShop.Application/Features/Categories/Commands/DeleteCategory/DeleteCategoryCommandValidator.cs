using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    private readonly IQueryRepository<Category> _categoryRepository;

    public DeleteCategoryCommandValidator(IQueryRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Kateqoriya ID-si boş ola bilməz")
            .MustAsync(CategoryExists)
            .WithMessage("Kateqoriya tapılmadı");
    }

    private async Task<bool> CategoryExists(Guid id, CancellationToken cancellationToken)
    {
        return await _categoryRepository.AnyAsync(c => c.Id == id, cancellationToken);
    }
}

