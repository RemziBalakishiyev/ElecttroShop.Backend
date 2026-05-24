using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryQueryRepository _categoryRepository;
    private readonly IQueryRepository<Domain.Entities.Category> _parentCategoryRepository;
    private readonly IDiscountCalculationService _discountCalculationService;

    public GetCategoriesQueryHandler(
        ICategoryQueryRepository categoryRepository,
        IQueryRepository<Domain.Entities.Category> parentCategoryRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _categoryRepository = categoryRepository;
        _parentCategoryRepository = parentCategoryRepository;
        _discountCalculationService = discountCalculationService;
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
            request.IncludeAll,
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

        var categoryDtos = new List<CategoryDto>();

        // Hər kateqoriya üçün endirim faizini hesabla
        foreach (var category in categories)
        {
            var discountPercent = await _discountCalculationService.GetCategoryDiscountPercentAsync(
                category.Id,
                cancellationToken);

            categoryDtos.Add(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ParentId = category.ParentId,
                ParentName = category.ParentId.HasValue && parents.ContainsKey(category.ParentId.Value)
                    ? parents[category.ParentId.Value]
                    : null,
                DiscountPercent = discountPercent,
                CreatedAt = category.CreatedAtUtc
            });
        }

        return PagedResult<CategoryDto>.Success(categoryDtos, request.Page, request.PageSize, totalCount);
    }
}

