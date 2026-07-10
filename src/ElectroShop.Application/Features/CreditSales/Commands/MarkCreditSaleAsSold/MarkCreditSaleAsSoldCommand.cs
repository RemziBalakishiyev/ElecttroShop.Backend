using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Commands.MarkCreditSaleAsSold;

public record MarkCreditSaleAsSoldCommand : MarkCreditSaleAsSoldRequest, IRequest<Result<CreditSaleDetailDto>>
{
    public Guid Id { get; init; }
}
