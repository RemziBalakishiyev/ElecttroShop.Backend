using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using FluentValidation;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Queries.GetCreditSalesSummary;

public class GetCreditSalesSummaryQueryValidator : AbstractValidator<GetCreditSalesSummaryQuery>
{
    public GetCreditSalesSummaryQueryValidator()
    {
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .When(x => x.Month.HasValue);

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2000)
            .When(x => x.Year.HasValue);
    }
}

public class GetCreditSalesSummaryQueryHandler : IRequestHandler<GetCreditSalesSummaryQuery, Result<CreditSaleSummaryDto>>
{
    private readonly ICreditSaleQueryRepository _creditSaleQueryRepository;

    public GetCreditSalesSummaryQueryHandler(ICreditSaleQueryRepository creditSaleQueryRepository)
    {
        _creditSaleQueryRepository = creditSaleQueryRepository;
    }

    public async Task<Result<CreditSaleSummaryDto>> Handle(
        GetCreditSalesSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var (fromDate, toDate) = ResolveDateRange(request);

        var aggregate = await _creditSaleQueryRepository.GetSummaryAsync(fromDate, toDate, cancellationToken);

        return Result.Success(new CreditSaleSummaryDto
        {
            PendingCount = aggregate.PendingCount,
            OverdueCount = aggregate.OverdueCount,
            SoldCount = aggregate.SoldCount,
            CancelledCount = aggregate.CancelledCount,
            TotalDebtAmount = aggregate.TotalDebtAmount,
            TotalPendingDebtAmount = aggregate.TotalPendingDebtAmount,
            TotalOverdueDebtAmount = aggregate.TotalOverdueDebtAmount,
            TotalSoldAmount = aggregate.TotalSoldAmount,
            TotalExpectedProfit = aggregate.TotalExpectedProfit,
            TotalNetProfit = aggregate.TotalNetProfit
        });
    }

    private static (DateTime? FromDate, DateTime? ToDate) ResolveDateRange(GetCreditSalesSummaryQuery request)
    {
        if (request.FromDate.HasValue || request.ToDate.HasValue)
            return (request.FromDate, request.ToDate);

        if (request.Month.HasValue && request.Year.HasValue)
        {
            var from = new DateTime(request.Year.Value, request.Month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = from.AddMonths(1).AddTicks(-1);
            return (from, to);
        }

        if (request.Month.HasValue || request.Year.HasValue)
        {
            var year = request.Year ?? DateTime.UtcNow.Year;
            var month = request.Month ?? DateTime.UtcNow.Month;
            var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = from.AddMonths(1).AddTicks(-1);
            return (from, to);
        }

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);
        return (monthStart, monthEnd);
    }
}
