using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Queries.GetCreditSaleById;

public record GetCreditSaleByIdQuery(Guid Id) : IRequest<Result<CreditSaleDetailDto>>;
