using Ecommerce.Core.Entities;
using Ecommerce.Core.Pagination;

namespace Ecommerce.Core.Interfaces;
public interface IProductRepository : IBaseRepository<Product,int>
{
    Task<PaginatedList<Product>> GetALLProductsAsync(int PageNumber ,int PageSize
        , string? SearchName,string? SearchCategory, string? SortColumn , string SortDirection = "asc", 
        int minPrice = 0, int maxPrice = int.MaxValue , CancellationToken cancellationToken = default);
}

