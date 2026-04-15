using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Errors;
public static class OrderErrors
{
    public static Error OrderNotFound => new(
        "Order.NotFound", "The order was not found.",StatusCodes.Status404NotFound);
    public static Error OrderAlreadyProcessed => new(
        "Order.AlreadyProcessed","The order has already been processed and cannot be modified.",StatusCodes.Status400BadRequest);
    public static Error EmptyCart => new(
        "Order.EmptyCart","there is no items in this cart" , StatusCodes.Status400BadRequest);
    public static Error InsufficientStock => new(
        "Order.InsufficientStock","There is insufficient stock for one or more items in the order." , StatusCodes.Status400BadRequest);
}
