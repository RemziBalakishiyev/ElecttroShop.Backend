using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Sales.Common;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Queries.GetSaleById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, Result<SaleDetailDto>>
{
    private readonly ISaleQueryRepository _saleQueryRepository;

    public GetSaleByIdQueryHandler(ISaleQueryRepository saleQueryRepository)
    {
        _saleQueryRepository = saleQueryRepository;
    }

    public async Task<Result<SaleDetailDto>> Handle(
        GetSaleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var sale = await _saleQueryRepository.GetSaleByIdAsync(request.Id, cancellationToken);
        if (sale is null)
            return Result.Failure<SaleDetailDto>(DomainErrors.Sale.NotFound(request.Id));

        return Result.Success(SaleMapper.ToDetailDto(sale));
    }
}
