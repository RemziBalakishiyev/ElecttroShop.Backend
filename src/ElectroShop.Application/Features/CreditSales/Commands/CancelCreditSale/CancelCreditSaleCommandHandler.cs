using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Services.CreditSales;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Commands.CancelCreditSale;

public class CancelCreditSaleCommandHandler : IRequestHandler<CancelCreditSaleCommand, Result>
{
    private readonly ICreditSaleService _creditSaleService;

    public CancelCreditSaleCommandHandler(ICreditSaleService creditSaleService)
    {
        _creditSaleService = creditSaleService;
    }

    public Task<Result> Handle(CancelCreditSaleCommand request, CancellationToken cancellationToken)
    {
        return _creditSaleService.CancelAsync(request.Id, cancellationToken);
    }
}
