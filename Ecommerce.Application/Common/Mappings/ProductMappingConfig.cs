using Ecommerce.Application.Contracts.Products;
using Ecommerce.Core.Pagination;

namespace Ecommerce.Application.Common.Mappings;
public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.CategoryName, src => src.Category.Name);

        config.NewConfig<PaginatedList<Product>, PaginatedList<ProductResponse>>()
            .MapWith(src => new PaginatedList<ProductResponse>(
                src.Items.Adapt<List<ProductResponse>>(),
                src.TotalCount,
                src.PageNumber,
                src.PageSize
            ));
    }
}
