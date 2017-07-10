using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Services.Es
{
    public interface ICategoryTreeConverter
    {
        Category ToCategory(ConverterContext context, Product product);
    }
}