using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.UpdateCategoryAttribute;

public class UpdateCategoryAttributeCommandHandler 
    : IRequestHandler<UpdateCategoryAttributeCommand, Result<CategoryAttributeDto>>
{
    private readonly IWriteRepository<CategoryAttribute> _attributeRepository;
    private readonly ICategoryQueryRepository _categoryQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryAttributeCommandHandler(
        IWriteRepository<CategoryAttribute> attributeRepository,
        ICategoryQueryRepository categoryQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _attributeRepository = attributeRepository;
        _categoryQueryRepository = categoryQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryAttributeDto>> Handle(
        UpdateCategoryAttributeCommand request,
        CancellationToken cancellationToken)
    {
        var attribute = await _categoryQueryRepository.GetCategoryAttributeWithValuesAsync(
            request.Id, 
            cancellationToken);

        if (attribute is null)
        {
            return DomainErrors.Category.NotFound(request.Id);
        }

        // Atributu yenilə
        attribute.Update(
            request.Name,
            request.DisplayName,
            request.AttributeType,
            request.IsRequired,
            request.DisplayOrder
        );

        // Dəyərlər ayrıca endpoint-lərlə idarə olunur, burada yalnız atribut məlumatları yenilənir

        _attributeRepository.Update(attribute);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Yenilənmiş atributu yüklə
        var updatedAttribute = await _categoryQueryRepository.GetCategoryAttributeWithValuesAsync(
            attribute.Id, 
            cancellationToken);

        if (updatedAttribute == null)
        {
            return DomainErrors.Category.NotFound(attribute.Id);
        }

        var attributeDto = new CategoryAttributeDto
        {
            Id = updatedAttribute.Id,
            Name = updatedAttribute.Name,
            DisplayName = updatedAttribute.DisplayName,
            AttributeType = updatedAttribute.AttributeType,
            IsRequired = updatedAttribute.IsRequired,
            DisplayOrder = updatedAttribute.DisplayOrder,
            Values = updatedAttribute.Values.Select(v => new CategoryAttributeValueDto
            {
                Id = v.Id,
                Value = v.Value,
                DisplayValue = v.DisplayValue,
                DisplayOrder = v.DisplayOrder,
                ColorCode = v.ColorCode
            }).OrderBy(v => v.DisplayOrder).ToList()
        };

        return Result.Success(attributeDto);
    }
}

