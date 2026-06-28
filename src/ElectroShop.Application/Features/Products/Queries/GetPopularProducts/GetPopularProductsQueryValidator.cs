using FluentValidation;

namespace ElectroShop.Application.Features.Products.Queries.GetPopularProducts;

public class GetPopularProductsQueryValidator : AbstractValidator<GetPopularProductsQuery>
{
    public GetPopularProductsQueryValidator()
    {
    }
}
