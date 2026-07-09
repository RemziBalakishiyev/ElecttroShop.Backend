using ElectroShop.Application.DTOs;

namespace ElectroShop.Application.Services.Reports;

public interface IReportsService
{
    Task<MonthlySalesReportDto> GetMonthlySalesReportAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
}
