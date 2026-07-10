using FluentValidation;

namespace ElectroShop.Application.Features.CreditSales.Commands.MarkCreditSaleAsSold;

public class MarkCreditSaleAsSoldCommandValidator : AbstractValidator<MarkCreditSaleAsSoldCommand>
{
    public MarkCreditSaleAsSoldCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.PaymentDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.PaymentDate.HasValue)
            .WithMessage("Ödəniş tarixi gələcək ola bilməz");

        RuleFor(x => x.SoldDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.SoldDate.HasValue)
            .WithMessage("Satış tarixi gələcək ola bilməz");
    }
}
