using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoryAttributes;

public record GetCategoryAttributesQuery(Guid CategoryId) : IRequest<Result<List<CategoryAttributeDto>>>;



