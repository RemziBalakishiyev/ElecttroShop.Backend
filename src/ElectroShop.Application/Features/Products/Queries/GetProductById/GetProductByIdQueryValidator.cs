using FluentValidation;

namespace ElectroShop.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// GetProductByIdQuery üçün Validator
/// </summary>
public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");
    }
}

