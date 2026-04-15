using Ecommerce.Application.Contracts.Categories;

namespace Ecommerce.Application.Common.Mappings;
public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryResponse>()
              .Map(dest => dest.Name, src => src.Name)
              .Map(dest => dest.ProductsCount, src => src.Products != null ? src.Products.Count() : 0);
    }
}
