using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.DeleteCategoryAttribute;

public class DeleteCategoryAttributeCommandHandler 
    : IRequestHandler<DeleteCategoryAttributeCommand, Result>
{
    private readonly IWriteRepository<CategoryAttribute> _attributeRepository;
    private readonly IQueryRepository<CategoryAttribute> _attributeQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryAttributeCommandHandler(
        IWriteRepository<CategoryAttribute> attributeRepository,
        IQueryRepository<CategoryAttribute> attributeQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _attributeRepository = attributeRepository;
        _attributeQueryRepository = attributeQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteCategoryAttributeCommand request,
        CancellationToken cancellationToken)
    {
        var attribute = await _attributeQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (attribute is null)
        {
            return Result.Failure(DomainErrors.Category.NotFound(request.Id));
        }

        attribute.MarkDeleted();
        _attributeRepository.Update(attribute);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

