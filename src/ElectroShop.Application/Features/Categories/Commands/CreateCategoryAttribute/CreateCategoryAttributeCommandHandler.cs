using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.CreateCategoryAttribute;

public class CreateCategoryAttributeCommandHandler 
    : IRequestHandler<CreateCategoryAttributeCommand, Result<CategoryAttributeDto>>
{
    private readonly IWriteRepository<CategoryAttribute> _attributeRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryAttributeCommandHandler(
        IWriteRepository<CategoryAttribute> attributeRepository,
        IQueryRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _attributeRepository = attributeRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryAttributeDto>> Handle(
        CreateCategoryAttributeCommand request,
        CancellationToken cancellationToken)
    {
        // Kateqoriyanın mövcud olduğunu yoxla
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            return DomainErrors.Category.NotFound(request.CategoryId);
        }

        // Atribut yarat
        var attribute = CategoryAttribute.Create(
            request.CategoryId,
            request.Name,
            request.DisplayName,
            request.AttributeType,
            request.IsRequired,
            request.DisplayOrder
        );

        await _attributeRepository.AddAsync(attribute, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var attributeDto = new CategoryAttributeDto
        {
            Id = attribute.Id,
            Name = attribute.Name,
            DisplayName = attribute.DisplayName,
            AttributeType = attribute.AttributeType,
            IsRequired = attribute.IsRequired,
            DisplayOrder = attribute.DisplayOrder,
            Values = [] // Dəyərlər ayrıca əlavə ediləcək
        };

        return Result.Success(attributeDto);
    }
}

