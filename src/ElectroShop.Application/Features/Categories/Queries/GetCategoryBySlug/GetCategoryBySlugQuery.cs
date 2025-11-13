using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoryBySlug;

public record GetCategoryBySlugQuery(string Slug) : IRequest<Result<CategoryDto>>;

