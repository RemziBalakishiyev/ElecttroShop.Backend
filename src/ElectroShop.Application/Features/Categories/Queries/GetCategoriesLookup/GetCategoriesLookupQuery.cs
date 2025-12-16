using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoriesLookup;

/// <summary>
/// Kateqoriyalar üçün lookup query - Key-Value formatında
/// Cache management ilə
/// </summary>
public record GetCategoriesLookupQuery : IRequest<Result<LookupResponse>>;

