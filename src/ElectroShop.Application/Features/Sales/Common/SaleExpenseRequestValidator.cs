using ElectroShop.Application.DTOs;
using FluentValidation;

namespace ElectroShop.Application.Features.Sales.Common;

public class SaleExpenseRequestValidator : AbstractValidator<SaleExpenseRequestDto>
{
    public SaleExpenseRequestValidator()
    {
        RuleFor(x => x.ExpenseType)
            .IsInEnum()
            .WithMessage("Xərc tipi etibarlı deyil");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Xərc məbləği mənfi ola bilməz");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
