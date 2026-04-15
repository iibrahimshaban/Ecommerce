namespace Ecommerce.Application.Contracts.Categories;
public record CategoryResponse(
    int Id,
    string Name,
    int ProductsCount
    );
