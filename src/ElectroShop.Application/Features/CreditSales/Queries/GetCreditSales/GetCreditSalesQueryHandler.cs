using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.CreditSales.Common;
using ElectroShop.Domain.Enums;
using FluentValidation;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Queries.GetCreditSales;

public class GetCreditSalesQueryValidator : AbstractValidator<GetCreditSalesQuery>
{
    public GetCreditSalesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}

public class GetCreditSalesQueryHandler : IRequestHandler<GetCreditSalesQuery, PagedResult<CreditSaleListItemDto>>
{
    private const int OverdueStatusFilter = 2;

    private readonly ICreditSaleQueryRepository _creditSaleQueryRepository;

    public GetCreditSalesQueryHandler(ICreditSaleQueryRepository creditSaleQueryRepository)
    {
        _creditSaleQueryRepository = creditSaleQueryRepository;
    }

    public async Task<PagedResult<CreditSaleListItemDto>> Handle(
        GetCreditSalesQuery request,
        CancellationToken cancellationToken)
    {
        CreditSaleStatus? status = null;
        bool? overdueOnly = null;

        if (request.Status.HasValue)
        {
            if (request.Status.Value == OverdueStatusFilter)
            {
                overdueOnly = true;
            }
            else if (Enum.IsDefined(typeof(CreditSaleStatus), request.Status.Value))
            {
                status = (CreditSaleStatus)request.Status.Value;
            }
        }

        var (creditSales, totalCount) = await _creditSaleQueryRepository.GetCreditSalesPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            status,
            overdueOnly,
            request.ProductSourceType,
            request.FromDate,
            request.ToDate,
            request.DueFromDate,
            request.DueToDate,
            cancellationToken);

        if (totalCount == 0)
            return PagedResult<CreditSaleListItemDto>.Empty(request.Page, request.PageSize);

        var items = creditSales.Select(c => CreditSaleMapper.ToListItemDto(c)).ToList();
        return PagedResult<CreditSaleListItemDto>.Success(items, request.Page, request.PageSize, totalCount);
    }
}
