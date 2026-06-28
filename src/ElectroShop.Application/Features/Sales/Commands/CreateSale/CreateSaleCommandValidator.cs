using FluentValidation;

namespace ElectroShop.Application.Features.Sales.Commands.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Satış qiyməti mənfi ola bilməz");

        When(x => x.ProductId.HasValue, () =>
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Məhsul ID-si tələb olunur");
        });

        When(x => !x.ProductId.HasValue, () =>
        {
            RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Manual satış üçün məhsul adı tələb olunur")
                .MaximumLength(300);

            RuleFor(x => x.CostPrice)
                .NotNull()
                .WithMessage("Manual satış üçün maya dəyəri tələb olunur")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maya dəyəri mənfi ola bilməz");

            RuleFor(x => x.ProductCode)
                .MaximumLength(50);

            RuleFor(x => x.CategoryName)
                .MaximumLength(200);
        });

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Note));

        RuleFor(x => x.SoldAt)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.SoldAt.HasValue)
            .WithMessage("Satış tarixi gələcək ola bilməz");
    }
}
