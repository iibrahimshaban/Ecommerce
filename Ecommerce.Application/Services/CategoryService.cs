using Ecommerce.Application.Contracts.Categories;

namespace Ecommerce.Application.Services;
public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.FindAll(x => true, cancellationToken,x => x.Products);

        var response = categories.Select(c => c.Adapt<CategoryResponse>());

        return response;

    }
    public async Task<Result<CategoryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.Find(x => x.Id == id, cancellationToken,x => x.Products);

        if (category == null)
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound);

        return Result.Success(category.Adapt<CategoryResponse>());

    }
    public async Task<Result<CategoryResponse>> CreateAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category { Name = request.Name};

        var nameExists = await _unitOfWork.Categories.AnyAsync(x => x.Name == request.Name, cancellationToken);

        if (nameExists)
            return Result.Failure<CategoryResponse>(CategoryErrors.duplicated);

        await _unitOfWork.Categories.AddAsync(category,cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(category.Adapt<CategoryResponse>());

    }

    public async Task<Result> UpdateAsync(int categoryId, CategoryRequest request, CancellationToken cancellationToken = default)
    {
       var nameExists = await  _unitOfWork.Categories.AnyAsync(x => x.Name == request.Name && x.Id != categoryId, cancellationToken);
        if (nameExists)
            return Result.Failure<CategoryResponse>(CategoryErrors.duplicated);

        var category = await  _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);

        if (category == null)
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound);

        category.Name = request.Name;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
