using ElectroShop.Application.Features.Sales.Queries.ExportMonthlySales;
using FluentValidation;

namespace ElectroShop.Application.Features.Sales.Queries.ExportMonthlySalesExcel;

public class ExportMonthlySalesExcelQueryValidator : AbstractValidator<ExportMonthlySalesExcelQuery>
{
    public ExportMonthlySalesExcelQueryValidator()
    {
        MonthlySalesExportValidationRules.Apply(this, x => x.Year, x => x.Month);
    }
}
