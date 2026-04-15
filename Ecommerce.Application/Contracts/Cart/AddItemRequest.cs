
namespace Ecommerce.Application.Contracts.Cart;
public record AddItemRequest(
    int ProductId,
    int Quantity
    );
