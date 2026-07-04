using ElectroShop.Application.Features.Sales.Common;
using FluentValidation;

namespace ElectroShop.Application.Features.Sales.Commands.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Satış qiyməti mənfi ola bilməz");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CostPrice.HasValue)
            .WithMessage("Maya dəyəri mənfi ola bilməz");

        RuleFor(x => x.ProductName)
            .MaximumLength(300)
            .When(x => !string.IsNullOrEmpty(x.ProductName));

        RuleFor(x => x.ProductCode)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.ProductCode));

        RuleFor(x => x.CategoryName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.CategoryName));

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Note));

        RuleFor(x => x.SoldAt)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.SoldAt.HasValue)
            .WithMessage("Satış tarixi gələcək ola bilməz");

        RuleForEach(x => x.Expenses)
            .SetValidator(new SaleExpenseRequestValidator())
            .When(x => x.Expenses is not null);
    }
}
