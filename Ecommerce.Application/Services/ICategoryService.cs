using Ecommerce.Application.Contracts.Categories;

namespace Ecommerce.Application.Services;
public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<CategoryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<CategoryResponse>> CreateAsync(CategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int categoryId, CategoryRequest request, CancellationToken cancellationToken = default);
}
