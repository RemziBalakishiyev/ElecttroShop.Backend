using ElectroShop.Application.DTOs;

namespace ElectroShop.Application.Services.Sales;

public interface ISalesExportService
{
    byte[] GenerateExcel(MonthlySalesReportDto report);
    byte[] GeneratePdf(MonthlySalesReportDto report);
}
