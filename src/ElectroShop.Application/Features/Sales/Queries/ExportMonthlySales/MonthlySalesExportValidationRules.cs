using System.Linq.Expressions;
using FluentValidation;

namespace ElectroShop.Application.Features.Sales.Queries.ExportMonthlySales;

internal static class MonthlySalesExportValidationRules
{
    public static void Apply<T>(
        AbstractValidator<T> validator,
        Expression<Func<T, int>> yearSelector,
        Expression<Func<T, int>> monthSelector)
    {
        validator.RuleFor(yearSelector)
            .InclusiveBetween(2000, 2100)
            .WithMessage("İl 2000 ilə 2100 arasında olmalıdır");

        validator.RuleFor(monthSelector)
            .InclusiveBetween(1, 12)
            .WithMessage("Ay 1 ilə 12 arasında olmalıdır");
    }
}
