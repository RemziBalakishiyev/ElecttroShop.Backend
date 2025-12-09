using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.DeleteCategoryAttributeValue;

public class DeleteCategoryAttributeValueCommandHandler 
    : IRequestHandler<DeleteCategoryAttributeValueCommand, Result>
{
    private readonly ICategoryQueryRepository _categoryQueryRepository;
    private readonly IWriteRepository<CategoryAttribute> _attributeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryAttributeValueCommandHandler(
        ICategoryQueryRepository categoryQueryRepository,
        IWriteRepository<CategoryAttribute> attributeRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryQueryRepository = categoryQueryRepository;
        _attributeRepository = attributeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteCategoryAttributeValueCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _categoryQueryRepository.GetAttributeAndValueByValueIdAsync(
            request.Id, 
            cancellationToken);
        
        if (result == null)
        {
            return Result.Failure(DomainErrors.Category.NotFound(request.Id));
        }

        var (parentAttribute, value) = result.Value;

        // Dəyəri atributdan sil
        parentAttribute.RemoveValue(value);

        _attributeRepository.Update(parentAttribute);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

