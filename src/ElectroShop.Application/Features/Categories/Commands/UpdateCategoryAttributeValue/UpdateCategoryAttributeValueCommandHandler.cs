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
    private readonly IWriteRepository<CategoryAttribute> _attributeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryAttributeValueCommandHandler(
        ICategoryQueryRepository categoryQueryRepository,
        IWriteRepository<CategoryAttribute> attributeRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryQueryRepository = categoryQueryRepository;
        _attributeRepository = attributeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryAttributeValueDto>> Handle(
        UpdateCategoryAttributeValueCommand request,
        CancellationToken cancellationToken)
    {
        // ICategoryQueryRepository-dən istifadə et (Persistence layer-də)
        // Amma bu Application layer-də ola bilməz. 
        // Ona görə də ICategoryQueryRepository-də metod əlavə etdik
        var result = await _categoryQueryRepository.GetAttributeAndValueByValueIdAsync(
            request.Id, 
            cancellationToken);

        if (result == null)
        {
            return DomainErrors.Category.NotFound(request.Id);
        }

        var (parentAttribute, value) = result.Value;

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

        _attributeRepository.Update(parentAttribute);
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

