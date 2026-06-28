using FluentValidation;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetMyProductRating;

public class GetMyProductRatingQueryValidator : AbstractValidator<GetMyProductRatingQuery>
{
    public GetMyProductRatingQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si tələb olunur");
    }
}
