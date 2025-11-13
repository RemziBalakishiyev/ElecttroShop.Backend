using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    Guid? ParentId = null,
    bool IncludeChildren = false) : IRequest<PagedResult<CategoryDto>>;

