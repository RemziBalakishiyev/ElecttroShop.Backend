using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrandsLookup;

/// <summary>
/// Brendlər üçün lookup query - Key-Value formatında
/// Cache management ilə
/// </summary>
public record GetBrandsLookupQuery : IRequest<Result<LookupResponse>>;

