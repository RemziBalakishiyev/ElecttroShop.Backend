using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services.CreditSales;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Commands.MarkCreditSaleAsSold;

public class MarkCreditSaleAsSoldCommandHandler : IRequestHandler<MarkCreditSaleAsSoldCommand, Result<CreditSaleDetailDto>>
{
    private readonly ICreditSaleService _creditSaleService;

    public MarkCreditSaleAsSoldCommandHandler(ICreditSaleService creditSaleService)
    {
        _creditSaleService = creditSaleService;
    }

    public Task<Result<CreditSaleDetailDto>> Handle(
        MarkCreditSaleAsSoldCommand request,
        CancellationToken cancellationToken)
    {
        var soldDate = request.PaymentDate ?? request.SoldDate;
        return _creditSaleService.MarkAsSoldAsync(request.Id, soldDate, cancellationToken);
    }
}
