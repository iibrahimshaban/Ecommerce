using Ecommerce.Application.Contracts.Pagination;
using Ecommerce.Application.Contracts.Products;
using Ecommerce.Core.Pagination;
using System.Threading;

namespace Ecommerce.Application.Services;
public interface IProductService
{
    Task<Result<PaginatedList<ProductResponse>>> GetProductsPagedAsync(RequestFilters filters,CancellationToken cancellationToken=default);
    Task<Result<ProductResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default);
    Task SendNewProductNotifications(int? productId = null, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int productId, ProductRequest request, CancellationToken cancellationToken = default);
    Task<Result> TogglePublishStatusAsync(int productId, CancellationToken cancellationToken = default);
}
