using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.ChangeStock;

public class ChangeStockCommandValidator : AbstractValidator<ChangeStockCommand>
{
    public ChangeStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.Operation)
            .IsInEnum()
            .WithMessage("Yanlış stok əməliyyatı");
    }
}

