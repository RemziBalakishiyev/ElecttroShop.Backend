using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Commands.CancelCreditSale;

public record CancelCreditSaleCommand(Guid Id) : IRequest<Result>;
