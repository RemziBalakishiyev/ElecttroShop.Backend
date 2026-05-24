using FluentValidation;

namespace ElectroShop.Application.Features.Discounts.Commands.CreateDiscount;

public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Dəstəklənməyən endirim tipi");

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

        // Type-ə görə müvafiq ID-nin olması lazımdır
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .When(x => x.Type == Domain.Enums.DiscountType.Product)
            .WithMessage("Məhsul endirimi üçün məhsul ID-si tələb olunur");

        RuleFor(x => x.BrandId)
            .NotEmpty()
            .When(x => x.Type == Domain.Enums.DiscountType.Brand)
            .WithMessage("Brend endirimi üçün brend ID-si tələb olunur");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .When(x => x.Type == Domain.Enums.DiscountType.Category)
            .WithMessage("Kateqoriya endirimi üçün kateqoriya ID-si tələb olunur");
    }
}









