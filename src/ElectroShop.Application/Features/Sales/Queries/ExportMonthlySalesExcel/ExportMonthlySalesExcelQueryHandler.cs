using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services.Sales;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Queries.ExportMonthlySalesExcel;

public class ExportMonthlySalesExcelQueryHandler
    : IRequestHandler<ExportMonthlySalesExcelQuery, Result<SalesExportFileDto>>
{
    private readonly ISalesReportService _salesReportService;
    private readonly ISalesExportService _salesExportService;

    public ExportMonthlySalesExcelQueryHandler(
        ISalesReportService salesReportService,
        ISalesExportService salesExportService)
    {
        _salesReportService = salesReportService;
        _salesExportService = salesExportService;
    }

    public async Task<Result<SalesExportFileDto>> Handle(
        ExportMonthlySalesExcelQuery request,
        CancellationToken cancellationToken)
    {
        var report = await _salesReportService.BuildMonthlyReportAsync(
            request.Year, request.Month, cancellationToken);

        var content = _salesExportService.GenerateExcel(report);
        var fileName = SalesMonthHelper.GetExportFileName(request.Year, request.Month, "xlsx");

        return Result.Success(new SalesExportFileDto
        {
            Content = content,
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            FileName = fileName
        });
    }
}
