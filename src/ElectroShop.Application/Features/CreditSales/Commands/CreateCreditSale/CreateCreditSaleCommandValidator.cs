using ElectroShop.Domain.Enums;

using FluentValidation;

using ElectroShop.Application.Features.Sales.Common;



namespace ElectroShop.Application.Features.CreditSales.Commands.CreateCreditSale;



public class CreateCreditSaleCommandValidator : AbstractValidator<CreateCreditSaleCommand>

{

    public CreateCreditSaleCommandValidator()

    {

        RuleFor(x => x.CustomerName)

            .MaximumLength(200)

            .When(x => !string.IsNullOrEmpty(x.CustomerName));



        RuleFor(x => x.CustomerPhone)

            .MaximumLength(50)

            .When(x => !string.IsNullOrEmpty(x.CustomerPhone));



        RuleFor(x => x.ProductSourceType)
            .IsInEnum()
            .WithMessage("Məhsul mənbə tipi düzgün deyil (Manual=1, SystemProduct=2)")
            .Must(x => x != default)
            .WithMessage("productSourceType seçilməlidir (Manual və ya SystemProduct)");

        RuleFor(x => x.CreditDate)
            .NotEqual(default(DateTime))
            .WithMessage("Nisyə tarixi tələb olunur");

        RuleFor(x => x.DueDate)
            .NotEqual(default(DateTime))
            .WithMessage("Son ödəniş tarixi tələb olunur");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.CreditDate.Date)
            .WithMessage("Son ödəniş tarixi nisyə tarixindən kiçik ola bilməz")
            .When(x => x.CreditDate != default && x.DueDate != default);

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Maya dəyəri mənfi ola bilməz");

        RuleFor(x => x.SalePrice)

            .GreaterThan(0)

            .WithMessage("Satış qiyməti 0-dan böyük olmalıdır")

            .When(x => x.ProductSourceType == CreditSaleProductSource.Manual);



        When(x => x.ProductSourceType == CreditSaleProductSource.SystemProduct, () =>

        {

            RuleFor(x => x.SalePrice)

                .GreaterThanOrEqualTo(0)

                .WithMessage("Satış qiyməti mənfi ola bilməz");

        });



        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.Note)

            .MaximumLength(1000)

            .When(x => !string.IsNullOrEmpty(x.Note));



        RuleForEach(x => x.Expenses)

            .SetValidator(new SaleExpenseRequestValidator())

            .When(x => x.Expenses is not null);



        When(x => x.ProductSourceType == CreditSaleProductSource.SystemProduct, () =>

        {

            RuleFor(x => x.ProductId)

                .NotEmpty()

                .WithMessage("Sistem məhsulu üçün ProductId tələb olunur");

        });



        When(x => x.ProductSourceType == CreditSaleProductSource.Manual, () =>

        {

            RuleFor(x => x.ProductName)

                .NotEmpty()

                .WithMessage("Manual nisyə üçün məhsul adı tələb olunur")

                .MaximumLength(300);



            RuleFor(x => x.Sku)

                .MaximumLength(50)

                .When(x => !string.IsNullOrEmpty(x.Sku));

        });

    }

}


