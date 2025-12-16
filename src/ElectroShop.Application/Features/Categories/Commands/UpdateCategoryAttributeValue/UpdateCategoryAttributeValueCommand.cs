using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.UpdateCategoryAttributeValue;

public record UpdateCategoryAttributeValueCommand : IRequest<Result<CategoryAttributeValueDto>>
{
    public Guid Id { get; init; }
    public string Value { get; init; } = string.Empty;
    public string? DisplayValue { get; init; }
    public int DisplayOrder { get; init; } = 0;
    public string? ColorCode { get; init; }
}



