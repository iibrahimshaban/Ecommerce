using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Errors;
public static class ProductErrors
{
    public static readonly Error NotFound =
    new("Product.ProductNotFound", "can't find product with given id", StatusCodes.Status404NotFound);

    public static readonly Error duplicated =
    new("Product.Productduplicated", "there are already a product with same description ", StatusCodes.Status409Conflict);
}
