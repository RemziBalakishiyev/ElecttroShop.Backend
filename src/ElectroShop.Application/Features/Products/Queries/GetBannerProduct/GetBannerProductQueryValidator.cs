using FluentValidation;

namespace ElectroShop.Application.Features.Products.Queries.GetBannerProduct;

public class GetBannerProductQueryValidator : AbstractValidator<GetBannerProductQuery>
{
    public GetBannerProductQueryValidator()
    {
        // No validation needed for empty query
    }
}


