using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    Guid? ParentId = null,
    string? Slug = null) : IRequest<Result<CategoryDto>>;

