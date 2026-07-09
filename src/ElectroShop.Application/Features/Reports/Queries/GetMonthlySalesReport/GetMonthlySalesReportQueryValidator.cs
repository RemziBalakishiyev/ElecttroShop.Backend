using ElectroShop.Application.Features.Sales.Queries.ExportMonthlySales;
using FluentValidation;

namespace ElectroShop.Application.Features.Reports.Queries.GetMonthlySalesReport;

public class GetMonthlySalesReportQueryValidator : AbstractValidator<GetMonthlySalesReportQuery>
{
    public GetMonthlySalesReportQueryValidator()
    {
        MonthlySalesExportValidationRules.Apply(this, x => x.Year, x => x.Month);
    }
}
