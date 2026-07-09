using ElectroShop.Application.Features.Sales.Queries.ExportMonthlySales;
using FluentValidation;

namespace ElectroShop.Application.Features.Sales.Queries.ExportMonthlySalesPdf;

public class ExportMonthlySalesPdfQueryValidator : AbstractValidator<ExportMonthlySalesPdfQuery>
{
    public ExportMonthlySalesPdfQueryValidator()
    {
        MonthlySalesExportValidationRules.Apply(this, x => x.Year, x => x.Month);
    }
}
