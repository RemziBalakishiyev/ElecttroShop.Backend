using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Commands.UpdateCreditSale;

public record UpdateCreditSaleCommand : UpdateCreditSaleRequest, IRequest<Result<CreditSaleDetailDto>>
{
    public Guid Id { get; init; }
}
