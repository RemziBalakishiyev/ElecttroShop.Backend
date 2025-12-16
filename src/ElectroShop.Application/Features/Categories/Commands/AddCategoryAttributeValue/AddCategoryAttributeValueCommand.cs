using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.AddCategoryAttributeValue;

public record AddCategoryAttributeValueCommand : IRequest<Result<CategoryAttributeValueDto>>
{
    public Guid CategoryAttributeId { get; init; }
    public string Value { get; init; } = string.Empty;
    public string? DisplayValue { get; init; }
    public int DisplayOrder { get; init; } = 0;
    public string? ColorCode { get; init; }
}



