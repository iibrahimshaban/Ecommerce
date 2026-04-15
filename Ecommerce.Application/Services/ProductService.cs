using Ecommerce.Application.Common.Helpers;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Contracts.Pagination;
using Ecommerce.Application.Contracts.Products;
using Ecommerce.Core.Pagination;
using Ecommerce.Shared.Authorization;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Services;
public class ProductService(IUnitOfWork unitOfWork , IUserRepository userRepository , IEmailSender emailSender , IHttpContextAccessor httpContextAccessor) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<Result<PaginatedList<ProductResponse>>> GetProductsPagedAsync(RequestFilters filters,CancellationToken cancellationToken=default)
    {
        if (filters.PageSize > 20)
            return Result.Failure<PaginatedList<ProductResponse>>(FiltersErrors.InvalidPageSize);

        var list = await _unitOfWork.Products
            .GetALLProductsAsync(filters.PageNumber, filters.PageSize,filters.SearchName,filters.SearchCategory,
            filters.SortColumn ,filters.SortDirection ,filters.MinPrice,filters.MaxPrice, cancellationToken);

        return Result.Success(list.Adapt<PaginatedList<ProductResponse>>());
    }
    public async Task<Result<ProductResponse>> GetByIdAsync(int id,CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.Find(x => x.Id == id , cancellationToken,y => y.Category);

        if (product == null || !product.IsPublished)
            return Result.Failure<ProductResponse>(ProductErrors.NotFound);

        return Result.Success(product.Adapt<ProductResponse>());
    }
    public async Task<Result<ProductResponse>> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        var Doublicated = await _unitOfWork.Products.AnyAsync(x => x.Name == request.Name && x.Description == request.Description, cancellationToken);

        if (Doublicated)
            return Result.Failure<ProductResponse>(ProductErrors.duplicated);

        var product = request.Adapt<Product>();

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Adapt<ProductResponse>());
    }
    public async Task SendNewProductNotifications(int? productId = null, CancellationToken cancellationToken = default)
    {
        IEnumerable<Product> products = [];

        if (productId.HasValue)
        {
            var product = await _unitOfWork.Products.Find(x => x.Id == productId && x.IsPublished);
            products = [product!];
        }
        else
        {
            products = await _unitOfWork.Products
                .FindAll(x => x.PublishedAt == DateOnly.FromDateTime(DateTime.UtcNow) && x.IsPublished);
        }

        var Users = await _userRepository.GetUsersInRoleAsync(DefaultRoles.Member.Name, cancellationToken);

        var host = _httpContextAccessor.HttpContext?.Request.Headers.Host;


        foreach (var user in Users)
        {
            foreach (var product in products)
            {

                var Placeholders = new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName+" "+user.LastName },
                    {"{{pollTill}}",product.Name },
                    {"{{Stock}}",product.Stock.ToString() },
                    {"{{Description}}",product.Description },
                    {"{{url}}",$"https://localhost:7095/api/Products/{product.Id}" }
                };

                var body = EmailBodyBuilder.GenerateEmailBody("NewItemNotification", Placeholders);

                await _emailSender.SendEmailAsync(user.Email!, $"💥 Survay Basket : New Poll - {product.Name}", body, cancellationToken: cancellationToken);
            }

        }
    }
    public async Task<Result> TogglePublishStatusAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);

        if (product == null)
            return Result.Failure(ProductErrors.NotFound);

        product.IsPublished = !product.IsPublished;
        
        if (product.IsPublished)
        {
            product.PublishedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            BackgroundJob.Enqueue(() => SendNewProductNotifications(productId,cancellationToken));
        }       
        else
            product.RevokedAt = DateOnly.FromDateTime(DateTime.UtcNow);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> UpdateAsync(int productId, ProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);

        if (product == null)
            return Result.Failure(ProductErrors.NotFound);

        var Doublicated = await _unitOfWork.Products
            .AnyAsync(x => x.Id != productId && (x.Name == request.Name && x.Description == request.Description), cancellationToken);
        
        if (Doublicated)
            return Result.Failure(ProductErrors.duplicated);

        request.Adapt(product);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
