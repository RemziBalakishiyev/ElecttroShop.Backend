using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IWriteRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Category> _categoryQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(
        IWriteRepository<Category> categoryRepository,
        IQueryRepository<Category> categoryQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _categoryQueryRepository = categoryQueryRepository;
        _unitOfWork = unitOfWork;
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

        return Result.Success();
    }
}

