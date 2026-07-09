using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services.Sales;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Queries.ExportMonthlySalesPdf;

public class ExportMonthlySalesPdfQueryHandler
    : IRequestHandler<ExportMonthlySalesPdfQuery, Result<SalesExportFileDto>>
{
    private readonly ISalesReportService _salesReportService;
    private readonly ISalesExportService _salesExportService;

    public ExportMonthlySalesPdfQueryHandler(
        ISalesReportService salesReportService,
        ISalesExportService salesExportService)
    {
        _salesReportService = salesReportService;
        _salesExportService = salesExportService;
    }

    public async Task<Result<SalesExportFileDto>> Handle(
        ExportMonthlySalesPdfQuery request,
        CancellationToken cancellationToken)
    {
        var report = await _salesReportService.BuildMonthlyReportAsync(
            request.Year, request.Month, cancellationToken);

        var content = _salesExportService.GeneratePdf(report);
        var fileName = SalesMonthHelper.GetExportFileName(request.Year, request.Month, "pdf");

        return Result.Success(new SalesExportFileDto
        {
            Content = content,
            ContentType = "application/pdf",
            FileName = fileName
        });
    }
}
