using Ecommerce.Core.Pagination;
using Ecommerce.Infrastructure.Extensions;
using System.Linq.Dynamic.Core;


namespace Ecommerce.Infrastructure.Repositories;
public class ProductRepository(ApplicationDbContext context) : BaseRepository<Product,int>(context), IProductRepository
{
    public async Task<PaginatedList<Product>> GetALLProductsAsync(int PageNumber, int PageSize, string? SearchName, string? searchCategory,
        string? SortColumn, string SortDirection = "asc", int minPrice = 0, int maxPrice = int.MaxValue, CancellationToken cancellationToken = default)
    {

        var query = _context.Products
            .Where(x => x.IsPublished)
            .Include(x => x.Category)
            .AsQueryable();

        

        if (!string.IsNullOrEmpty(SearchName))
        {
            query = query.Where(x => x.Name.Contains(SearchName, StringComparison.CurrentCultureIgnoreCase));
        }
        if (!string.IsNullOrEmpty(searchCategory))
        {
            query = query.Where(x => x.Category.Name.Contains(searchCategory, StringComparison.CurrentCultureIgnoreCase));
        }

        if (!string.IsNullOrEmpty(SortColumn))
        {
            query = query.OrderBy($"{SortColumn} {SortDirection}");
        }

        query = query.Where(p => p.Price >= minPrice);
        query = query.Where(p => p.Price <= maxPrice);

        var products = await query.ToPaginatedListAsync(PageNumber, PageSize, cancellationToken);

        return products;
    }
}
