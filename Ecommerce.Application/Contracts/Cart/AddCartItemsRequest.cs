
namespace Ecommerce.Application.Contracts.Cart;
public record AddCartItemsRequest(
    List<AddItemRequest> Items
    );
