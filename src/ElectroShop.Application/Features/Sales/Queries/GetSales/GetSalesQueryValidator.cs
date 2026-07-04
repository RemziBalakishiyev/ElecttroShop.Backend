using FluentValidation;

namespace ElectroShop.Application.Features.Sales.Queries.GetSales;

public class GetSalesQueryValidator : AbstractValidator<GetSalesQuery>
{
    public GetSalesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("Bitmə tarixi başlanğıc tarixindən əvvəl ola bilməz");

        RuleFor(x => x.MaxProfit)
            .GreaterThanOrEqualTo(x => x.MinProfit)
            .When(x => x.MinProfit.HasValue && x.MaxProfit.HasValue)
            .WithMessage("Maksimum mənfəət minimumdan kiçik ola bilməz");

        RuleFor(x => x.MaxExpense)
            .GreaterThanOrEqualTo(x => x.MinExpense)
            .When(x => x.MinExpense.HasValue && x.MaxExpense.HasValue)
            .WithMessage("Maksimum xərc minimumdan kiçik ola bilməz");
    }
}
