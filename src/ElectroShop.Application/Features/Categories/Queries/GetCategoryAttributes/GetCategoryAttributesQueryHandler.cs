using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoryAttributes;

public class GetCategoryAttributesQueryHandler 
    : IRequestHandler<GetCategoryAttributesQuery, Result<List<CategoryAttributeDto>>>
{
    private readonly ICategoryQueryRepository _categoryQueryRepository;

    public GetCategoryAttributesQueryHandler(ICategoryQueryRepository categoryQueryRepository)
    {
        _categoryQueryRepository = categoryQueryRepository;
    }

    public async Task<Result<List<CategoryAttributeDto>>> Handle(
        GetCategoryAttributesQuery request,
        CancellationToken cancellationToken)
    {
        var attributes = await _categoryQueryRepository.GetCategoryAttributesAsync(
            request.CategoryId, 
            cancellationToken);

        var attributeDtos = attributes.Select(attr => new CategoryAttributeDto
        {
            Id = attr.Id,
            Name = attr.Name,
            DisplayName = attr.DisplayName,
            AttributeType = attr.AttributeType,
            IsRequired = attr.IsRequired,
            DisplayOrder = attr.DisplayOrder,
            Values = attr.Values.Select(val => new CategoryAttributeValueDto
            {
                Id = val.Id,
                Value = val.Value,
                DisplayValue = val.DisplayValue,
                DisplayOrder = val.DisplayOrder,
                ColorCode = val.ColorCode
            }).OrderBy(v => v.DisplayOrder).ToList()
        }).OrderBy(a => a.DisplayOrder).ToList();

        return Result.Success(attributeDtos);
    }
}


