using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Queries.GetSaleById;

public record GetSaleByIdQuery(Guid Id) : IRequest<Result<SaleDetailDto>>;
