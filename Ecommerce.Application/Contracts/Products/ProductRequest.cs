namespace Ecommerce.Application.Contracts.Products;
public record ProductRequest(
    string Name,
    string Description,
    double Price,
    int Stock,
    int CategoryId
    );
