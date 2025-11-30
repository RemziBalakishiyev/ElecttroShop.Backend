using ElectroShop.Application.DTOs;
using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetPromotionalBrands;

/// <summary>
/// Promotional brendləri və hər brend üçün featured məhsulu əldə edir
/// </summary>
public record GetPromotionalBrandsQuery : IRequest<Result<List<PromotionalBrandDto>>>;

