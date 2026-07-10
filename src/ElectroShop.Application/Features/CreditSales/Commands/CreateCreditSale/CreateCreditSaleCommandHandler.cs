using ElectroShop.Application.Services.CreditSales;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Commands.CreateCreditSale;

public class CreateCreditSaleCommandHandler : IRequestHandler<CreateCreditSaleCommand, Result<CreditSaleDetailDto>>
{
    private readonly ICreditSaleService _creditSaleService;

    public CreateCreditSaleCommandHandler(ICreditSaleService creditSaleService)
    {
        _creditSaleService = creditSaleService;
    }

    public Task<Result<CreditSaleDetailDto>> Handle(
        CreateCreditSaleCommand request,
        CancellationToken cancellationToken)
    {
        return _creditSaleService.CreateAsync(request, cancellationToken);
    }
}
