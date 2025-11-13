using FluentValidation;

namespace ElectroShop.Application.Features.Customers.Queries.GetCustomerByEmail;

public class GetCustomerByEmailQueryValidator : AbstractValidator<GetCustomerByEmailQuery>
{
    public GetCustomerByEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-poçt ünvanı boş ola bilməz")
            .EmailAddress()
            .WithMessage("Yanlış e-poçt ünvanı formatı")
            .MaximumLength(200)
            .WithMessage("E-poçt ünvanı maksimum 200 simvol ola bilər");
    }
}

