using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.AddCategoryAttributeValue;

public class AddCategoryAttributeValueCommandHandler 
    : IRequestHandler<AddCategoryAttributeValueCommand, Result<CategoryAttributeValueDto>>
{
    private readonly IWriteRepository<CategoryAttribute> _attributeRepository;
    private readonly ICategoryQueryRepository _categoryQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryAttributeValueCommandHandler(
        IWriteRepository<CategoryAttribute> attributeRepository,
        ICategoryQueryRepository categoryQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _attributeRepository = attributeRepository;
        _categoryQueryRepository = categoryQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryAttributeValueDto>> Handle(
        AddCategoryAttributeValueCommand request,
        CancellationToken cancellationToken)
    {
        var attribute = await _categoryQueryRepository.GetCategoryAttributeWithValuesAsync(
            request.CategoryAttributeId, 
            cancellationToken);

        if (attribute is null)
        {
            return DomainErrors.Category.NotFound(request.CategoryAttributeId);
        }

        // Eyni dəyərin mövcud olub-olmadığını yoxla
        if (attribute.Values.Any(v => v.Value == request.Value))
        {
            return Result.Failure<CategoryAttributeValueDto>(
                Error.Conflict("CategoryAttributeValue.AlreadyExists", 
                    $"Bu dəyər artıq mövcuddur: {request.Value}"));
        }

        var value = CategoryAttributeValue.Create(
            attribute.Id,
            request.Value,
            request.DisplayValue,
            request.DisplayOrder,
            request.ColorCode
        );

        attribute.AddValue(value);
        _attributeRepository.Update(attribute);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var valueDto = new CategoryAttributeValueDto
        {
            Id = value.Id,
            Value = value.Value,
            DisplayValue = value.DisplayValue,
            DisplayOrder = value.DisplayOrder,
            ColorCode = value.ColorCode
        };

        return Result.Success(valueDto);
    }
}

