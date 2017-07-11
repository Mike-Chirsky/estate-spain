using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Services.Es.Converters
{

    public class DefaultCategoryTreeConverter : ICategoryTreeConverter
    {
        public DefaultCategoryTreeConverter()
        {
        }

        public virtual Category ToCategory(ConverterContext context, Product product)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var result = new Category();
            result.Id = product.Id;
            result.CatalogId = product.CatalogId;
            result.Code = context.Parent.Id !=null ? $"{context.Parent.Code}-{product.Sku}" : product.Sku;

            result.Name = product.Name;

            result.FullName = CreateFullName(context, product);

            result.ParentId = context.Parent.Id;
            result.TaxType = product.TaxType;

            result.SeoInfo = product.SeoInfo;
            
            //result.Outline = product.Outlines;
            result.SeoPath = context.Parent.Url + "/" + product.SeoInfo.Slug;
            result.Url = context.Parent.Url + "/" + product.SeoInfo.Slug;

            if (product.Images != null)
            {
                result.Images = product.Images.ToList();
                result.PrimaryImage = result.PrimaryImage;
            }

            if (product.Properties != null)
            {
                result.Properties = product.Properties.ToList();
            }

            // ES Properties
            result.ProductType = context.ProductType;
            result.Path = context.Path;

            result.Description = product.Description;
            result.Descriptions = product.Descriptions.ToList();

            result.Parent = context.Parent;

            return result;

        }

        protected virtual string CreateFullName(ConverterContext context, Product product)
        {
            // thrid level
            if (context.Parent.Parent != null && context.Parent.Parent.Id!=null)
                return $"{context.Parent.FullName} {product.Name}";

            // second level
            if (context.Parent.Id != null)
                return $"{product.Name} в {context.Parent.FullName}";

            return product.Name;
        }
    }
}