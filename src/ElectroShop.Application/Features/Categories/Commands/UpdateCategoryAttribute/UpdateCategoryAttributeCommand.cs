using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.UpdateCategoryAttribute;

public record UpdateCategoryAttributeCommand : IRequest<Result<CategoryAttributeDto>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string AttributeType { get; init; } = string.Empty;
    public bool IsRequired { get; init; } = false;
    public int DisplayOrder { get; init; } = 0;
}

