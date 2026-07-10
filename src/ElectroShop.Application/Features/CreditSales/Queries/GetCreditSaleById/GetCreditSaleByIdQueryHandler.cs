using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.CreditSales.Common;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Queries.GetCreditSaleById;

public class GetCreditSaleByIdQueryHandler : IRequestHandler<GetCreditSaleByIdQuery, Result<CreditSaleDetailDto>>
{
    private readonly ICreditSaleQueryRepository _creditSaleQueryRepository;

    public GetCreditSaleByIdQueryHandler(ICreditSaleQueryRepository creditSaleQueryRepository)
    {
        _creditSaleQueryRepository = creditSaleQueryRepository;
    }

    public async Task<Result<CreditSaleDetailDto>> Handle(
        GetCreditSaleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var creditSale = await _creditSaleQueryRepository.GetCreditSaleByIdAsync(request.Id, cancellationToken);
        if (creditSale is null)
            return Result.Failure<CreditSaleDetailDto>(DomainErrors.CreditSale.NotFound(request.Id));

        return Result.Success(CreditSaleMapper.ToDetailDto(creditSale));
    }
}
