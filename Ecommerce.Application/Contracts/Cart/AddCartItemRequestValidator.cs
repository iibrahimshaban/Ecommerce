namespace Ecommerce.Application.Contracts.Cart;
public class AddCartItemRequestValidator : AbstractValidator<AddCartItemsRequest>
{
    public AddCartItemRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item must be provided.");

        RuleForEach(x => x.Items)
            .SetValidator(new AddItemRequestValidator());
    }
}
