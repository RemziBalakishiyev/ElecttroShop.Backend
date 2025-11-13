using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoryBySlug;

public class GetCategoryBySlugQueryHandler : IRequestHandler<GetCategoryBySlugQuery, Result<CategoryDto>>
{
    private readonly IQueryRepository<Domain.Entities.Category> _categoryRepository;

    public GetCategoryBySlugQueryHandler(IQueryRepository<Domain.Entities.Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var slug = request.Slug?.ToLowerInvariant();

        var category = await _categoryRepository.FirstOrDefaultAsync(
            c => c.Slug == slug && !c.IsDeleted,
            cancellationToken);

        if (category is null)
        {
            return Result.Failure<CategoryDto>(
                Error.NotFound("Category.NotFoundBySlug", $"Slug '{request.Slug}' ilə kateqoriya tapılmadı"));
        }

        var parent = category.ParentId.HasValue
            ? await _categoryRepository.GetByIdAsync(category.ParentId.Value, cancellationToken)
            : null;

        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ParentId = category.ParentId,
            ParentName = parent?.Name,
            CreatedAt = category.CreatedAtUtc
        };

        return Result.Success(categoryDto);
    }
}


