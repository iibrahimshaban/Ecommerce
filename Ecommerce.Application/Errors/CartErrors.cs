using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Errors;
public static class CartErrors
{
    public static readonly Error NotFound =
    new("Cart.CartNotFound", "this user don't have a cart yet", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyExists =
    new("Cart.CartAlreadyExists", "there are already a cart for same user ", StatusCodes.Status409Conflict);

    public static readonly Error CreateFailed =
    new("Cart.CartCreateFailed", "can add the new items to the cart ", StatusCodes.Status400BadRequest);

    public static readonly Error UpdateFailed =
    new("Cart.CartUpdateFailed", "can not update the items quantity", StatusCodes.Status400BadRequest);

    public static readonly Error DeleteFailed =
    new("Cart.CartDeleteFailed", "can not delete the item from cart", StatusCodes.Status400BadRequest);
}
