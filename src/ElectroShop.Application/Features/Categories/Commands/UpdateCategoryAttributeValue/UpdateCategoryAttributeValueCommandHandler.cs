using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.UpdateCategoryAttributeValue;

public class UpdateCategoryAttributeValueCommandHandler 
    : IRequestHandler<UpdateCategoryAttributeValueCommand, Result<CategoryAttributeValueDto>>
{
    private readonly ICategoryQueryRepository _categoryQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryAttributeValueCommandHandler(
        ICategoryQueryRepository categoryQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryQueryRepository = categoryQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryAttributeValueDto>> Handle(
        UpdateCategoryAttributeValueCommand request,
        CancellationToken cancellationToken)
    {
        // Value'yu tracking ile al (update üçün)
        var value = await _categoryQueryRepository.GetCategoryAttributeValueForUpdateAsync(
            request.Id, 
            cancellationToken);

        if (value == null)
        {
            return DomainErrors.Category.NotFound(request.Id);
        }

        var parentAttribute = value.CategoryAttribute;

        // Eyni dəyərin başqa value-da olub-olmadığını yoxla
        if (parentAttribute.Values.Any(v => v.Value == request.Value && v.Id != request.Id))
        {
            return Result.Failure<CategoryAttributeValueDto>(
                Error.Conflict("CategoryAttributeValue.AlreadyExists", 
                    $"Bu dəyər artıq mövcuddur: {request.Value}"));
        }

        value.Update(
            request.Value,
            request.DisplayValue,
            request.DisplayOrder,
            request.ColorCode
        );

        // Value'yu repository üzerinden güncelle
        _categoryQueryRepository.UpdateCategoryAttributeValue(value);
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

