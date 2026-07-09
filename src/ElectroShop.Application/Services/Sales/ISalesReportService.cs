using ElectroShop.Application.DTOs;

namespace ElectroShop.Application.Services.Sales;

public interface ISalesReportService
{
    Task<MonthlySalesReportDto> BuildMonthlyReportAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
}
