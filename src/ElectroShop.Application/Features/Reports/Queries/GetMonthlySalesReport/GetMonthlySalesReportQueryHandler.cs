using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services.Reports;
using MediatR;

namespace ElectroShop.Application.Features.Reports.Queries.GetMonthlySalesReport;

public class GetMonthlySalesReportQueryHandler
    : IRequestHandler<GetMonthlySalesReportQuery, Result<MonthlySalesReportDto>>
{
    private readonly IReportsService _reportsService;

    public GetMonthlySalesReportQueryHandler(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    public async Task<Result<MonthlySalesReportDto>> Handle(
        GetMonthlySalesReportQuery request,
        CancellationToken cancellationToken)
    {
        var report = await _reportsService.GetMonthlySalesReportAsync(
            request.Year, request.Month, cancellationToken);

        return Result.Success(report);
    }
}
