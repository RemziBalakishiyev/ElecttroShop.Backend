using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IQueryRepository<Domain.Entities.Category> _categoryRepository;
    private readonly IDiscountCalculationService _discountCalculationService;

    public GetCategoryByIdQueryHandler(
        IQueryRepository<Domain.Entities.Category> categoryRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _categoryRepository = categoryRepository;
        _discountCalculationService = discountCalculationService;
    }

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FirstOrDefaultAsync(
            c => c.Id == request.Id && !c.IsDeleted,
            cancellationToken);

        if (category is null)
        {
            return DomainErrors.Category.NotFound(request.Id);
        }

        var parent = category.ParentId.HasValue
            ? await _categoryRepository.GetByIdAsync(category.ParentId.Value, cancellationToken)
            : null;

        // Endirim faizini hesabla
        var discountPercent = await _discountCalculationService.GetCategoryDiscountPercentAsync(
            category.Id,
            cancellationToken);

        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ParentId = category.ParentId,
            ParentName = parent?.Name,
            DiscountPercent = discountPercent,
            CreatedAt = category.CreatedAtUtc
        };

        return Result.Success(categoryDto);
    }
}


