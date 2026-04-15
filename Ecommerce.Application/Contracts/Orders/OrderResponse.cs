namespace Ecommerce.Application.Contracts.Orders;
public record OrderResponse(
    int Id,
    string UserId,
    double TotalPrice,
    DateTime CreatedAt,
    string Status,
    IEnumerable<OrderItemResponse> Items
    );
