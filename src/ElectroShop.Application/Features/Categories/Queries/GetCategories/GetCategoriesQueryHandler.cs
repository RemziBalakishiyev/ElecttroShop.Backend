using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryQueryRepository _categoryRepository;
    private readonly IQueryRepository<Domain.Entities.Category> _parentCategoryRepository;

    public GetCategoriesQueryHandler(
        ICategoryQueryRepository categoryRepository,
        IQueryRepository<Domain.Entities.Category> parentCategoryRepository)
    {
        _categoryRepository = categoryRepository;
        _parentCategoryRepository = parentCategoryRepository;
    }

    public async Task<PagedResult<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var (categories, totalCount) = await _categoryRepository.GetCategoriesPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.ParentId,
            request.IncludeChildren,
            cancellationToken);

        if (totalCount == 0)
        {
            return PagedResult<CategoryDto>.Empty(request.Page, request.PageSize);
        }

        var parentIds = categories
            .Where(c => c.ParentId.HasValue)
            .Select(c => c.ParentId!.Value)
            .Distinct()
            .ToList();

        var parents = new Dictionary<Guid, string>();
        foreach (var parentId in parentIds)
        {
            var parent = await _parentCategoryRepository.GetByIdAsync(parentId, cancellationToken);
            if (parent != null)
                parents[parentId] = parent.Name;
        }

        var categoryDtos = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            ParentId = c.ParentId,
            ParentName = c.ParentId.HasValue && parents.ContainsKey(c.ParentId.Value) 
                ? parents[c.ParentId.Value] 
                : null,
            CreatedAt = c.CreatedAtUtc
        }).ToList();

        return PagedResult<CategoryDto>.Success(categoryDtos, request.Page, request.PageSize, totalCount);
    }
}

