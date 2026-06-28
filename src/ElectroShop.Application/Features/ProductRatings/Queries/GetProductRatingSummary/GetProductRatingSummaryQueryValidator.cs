using FluentValidation;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetProductRatingSummary;

public class GetProductRatingSummaryQueryValidator : AbstractValidator<GetProductRatingSummaryQuery>
{
    public GetProductRatingSummaryQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si tələb olunur");
    }
}
