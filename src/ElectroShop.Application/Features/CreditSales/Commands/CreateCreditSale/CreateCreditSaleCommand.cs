using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Commands.CreateCreditSale;

public record CreateCreditSaleCommand : CreateCreditSaleRequest, IRequest<Result<CreditSaleDetailDto>>;
