using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly IWriteRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Category> _categoryQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILookupCacheInvalidator _lookupCacheInvalidator;

    public UpdateCategoryCommandHandler(
        IWriteRepository<Category> categoryRepository,
        IQueryRepository<Category> categoryQueryRepository,
        IUnitOfWork unitOfWork,
        ILookupCacheInvalidator lookupCacheInvalidator)
    {
        _categoryRepository = categoryRepository;
        _categoryQueryRepository = categoryQueryRepository;
        _unitOfWork = unitOfWork;
        _lookupCacheInvalidator = lookupCacheInvalidator;
    }

    public async Task<Result<CategoryDto>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return DomainErrors.Category.NotFound(request.Id);
        }

        Category? parent = null;
        if (request.ParentId.HasValue)
        {
            parent = await _categoryQueryRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
            {
                return DomainErrors.Category.NotFound(request.ParentId.Value);
            }
        }

        try
        {
            category.Update(request.Name, request.ParentId, request.Slug);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<CategoryDto>(Error.Validation("Category.InvalidData", ex.Message));
        }

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _lookupCacheInvalidator.InvalidateCategoriesLookup();

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

