using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IWriteRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Category> _categoryQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        IWriteRepository<Category> categoryRepository,
        IQueryRepository<Category> categoryQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _categoryQueryRepository = categoryQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryDto>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        Category? parent = null;
        if (request.ParentId.HasValue)
        {
            parent = await _categoryQueryRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
            {
                return DomainErrors.Category.NotFound(request.ParentId.Value);
            }
        }

        Category category;
        try
        {
            category = Category.Create(request.Name, request.ParentId, request.Slug);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<CategoryDto>(Error.Validation("Category.InvalidData", ex.Message));
        }

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

