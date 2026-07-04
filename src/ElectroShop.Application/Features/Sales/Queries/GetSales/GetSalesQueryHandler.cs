using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Sales.Common;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Queries.GetSales;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, PagedResult<SaleListItemDto>>
{
    private readonly ISaleQueryRepository _saleQueryRepository;

    public GetSalesQueryHandler(ISaleQueryRepository saleQueryRepository)
    {
        _saleQueryRepository = saleQueryRepository;
    }

    public async Task<PagedResult<SaleListItemDto>> Handle(
        GetSalesQuery request,
        CancellationToken cancellationToken)
    {
        var (sales, totalCount) = await _saleQueryRepository.GetSalesPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.CategoryId,
            request.ProductId,
            request.SaleSource,
            request.DateFrom,
            request.DateTo,
            request.MinProfit,
            request.MaxProfit,
            request.MinExpense,
            request.MaxExpense,
            cancellationToken);

        if (totalCount == 0)
            return PagedResult<SaleListItemDto>.Empty(request.Page, request.PageSize);

        var items = sales.Select(SaleMapper.ToListItemDto).ToList();
        return PagedResult<SaleListItemDto>.Success(items, request.Page, request.PageSize, totalCount);
    }
}
