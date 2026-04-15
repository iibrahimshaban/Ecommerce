namespace Ecommerce.Application.Contracts.Cart;
public record ItemResponse(
    int Id,
    int ProductId,
    string ProductName,
    string Description,
    double Price,
    int Quantity,
    double Total
    );
