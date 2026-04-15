namespace Ecommerce.Application.Contracts.Orders;
public record OrderItemResponse(
    int ProductId,
    string ProductName,
    double UnitPrice,
    int Quantity,
    double Subtotal
    );
