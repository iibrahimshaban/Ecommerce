namespace Ecommerce.Application.Contracts.Cart;
public record CartResponse(
    int CartId,
    string UserId,
    IEnumerable<ItemResponse> Items,
    double TotalAmount
    );
