using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.ProductRatings.Commands.UpdateProductRating;

public class UpdateProductRatingCommandValidator : AbstractValidator<UpdateProductRatingCommand>
{
    public UpdateProductRatingCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si tələb olunur");

        RuleFor(x => x.RatingValue)
            .InclusiveBetween(ProductRating.MinRatingValue, ProductRating.MaxRatingValue)
            .WithMessage("Reytinq 1 ilə 5 arasında olmalıdır");

        RuleFor(x => x.Comment)
            .MaximumLength(ProductRating.MaxCommentLength)
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}
