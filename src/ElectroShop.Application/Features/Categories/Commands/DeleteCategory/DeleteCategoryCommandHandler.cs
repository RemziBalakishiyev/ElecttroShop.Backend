using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IWriteRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Category> _categoryQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILookupCacheInvalidator _lookupCacheInvalidator;

    public DeleteCategoryCommandHandler(
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

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure(DomainErrors.Category.NotFound(request.Id));
        }

        category.MarkDeleted();

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _lookupCacheInvalidator.InvalidateCategoriesLookup();

        return Result.Success();
    }
}

