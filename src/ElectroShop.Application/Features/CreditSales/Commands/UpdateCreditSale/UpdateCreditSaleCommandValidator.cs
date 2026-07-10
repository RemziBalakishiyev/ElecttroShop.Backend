using FluentValidation;
using ElectroShop.Application.Features.Sales.Common;

namespace ElectroShop.Application.Features.CreditSales.Commands.UpdateCreditSale;

public class UpdateCreditSaleCommandValidator : AbstractValidator<UpdateCreditSaleCommand>
{
    public UpdateCreditSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CustomerName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.CustomerName));

        RuleFor(x => x.CustomerPhone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.CustomerPhone));

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Maya dəyəri mənfi ola bilməz");

        RuleFor(x => x.SalePrice)
            .GreaterThan(0)
            .WithMessage("Satış qiyməti 0-dan böyük olmalıdır");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.DueDate)
            .NotEqual(default(DateTime))
            .WithMessage("Son ödəniş tarixi tələb olunur");

        RuleFor(x => x.CreditDate)
            .NotEqual(default(DateTime))
            .WithMessage("Nisyə tarixi tələb olunur");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.CreditDate.Date)
            .WithMessage("Son ödəniş tarixi nisyə tarixindən kiçik ola bilməz")
            .When(x => x.CreditDate != default && x.DueDate != default);

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Note));

        RuleForEach(x => x.Expenses)
            .SetValidator(new SaleExpenseRequestValidator())
            .When(x => x.Expenses is not null);
    }
}
