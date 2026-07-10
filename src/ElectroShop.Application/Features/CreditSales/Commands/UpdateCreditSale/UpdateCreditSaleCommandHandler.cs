using ElectroShop.Application.Services.CreditSales;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Commands.UpdateCreditSale;

public class UpdateCreditSaleCommandHandler : IRequestHandler<UpdateCreditSaleCommand, Result<CreditSaleDetailDto>>
{
    private readonly ICreditSaleService _creditSaleService;

    public UpdateCreditSaleCommandHandler(ICreditSaleService creditSaleService)
    {
        _creditSaleService = creditSaleService;
    }

    public Task<Result<CreditSaleDetailDto>> Handle(
        UpdateCreditSaleCommand request,
        CancellationToken cancellationToken)
    {
        return _creditSaleService.UpdateAsync(request.Id, request, cancellationToken);
    }
}
