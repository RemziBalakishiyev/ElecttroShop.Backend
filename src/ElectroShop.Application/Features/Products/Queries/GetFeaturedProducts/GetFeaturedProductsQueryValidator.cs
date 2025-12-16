using FluentValidation;

namespace ElectroShop.Application.Features.Products.Queries.GetFeaturedProducts;

public class GetFeaturedProductsQueryValidator : AbstractValidator<GetFeaturedProductsQuery>
{
    public GetFeaturedProductsQueryValidator()
    {
        // No validation needed for empty query
    }
}




