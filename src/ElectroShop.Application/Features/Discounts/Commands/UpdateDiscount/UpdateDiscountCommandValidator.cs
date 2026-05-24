using FluentValidation;

namespace ElectroShop.Application.Features.Discounts.Commands.UpdateDiscount;

public class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
{
    public UpdateDiscountCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Endirim ID-si tələb olunur");

        RuleFor(x => x.Percent)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Endirim faizi 0-100 arasında olmalıdır");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Başlanğıc tarix tələb olunur");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("Bitmə tarixi başlanğıc tarixindən sonra olmalıdır");
    }
}









