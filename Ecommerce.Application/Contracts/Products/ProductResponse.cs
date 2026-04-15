namespace Ecommerce.Application.Contracts.Products;
public record ProductResponse(
    int Id,
    string Name ,
    string Description,
    double Price,
    int Stock,
    string CategoryName
    );
