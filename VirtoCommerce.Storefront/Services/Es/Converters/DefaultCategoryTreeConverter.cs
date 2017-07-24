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

        protected virtual void FillFromException(ConverterContext context, Category category)
        {
            if (context.ListExceptions != null)
            {
                var seoPath = category.SeoPath.Trim('/');
                var exception = context.ListExceptions.FirstOrDefault(x => x.SeoInfo != null && category.SeoPath != null ? x.SeoInfo.Slug.Replace("-slash-", "/") == seoPath : false);
                if (exception != null)
                {
                    category.FullName = exception.Name;
                    category.Description = exception.Description;
                    category.Descriptions = exception.Descriptions;
                    category.Properties = exception.Properties;
                    category.Images = exception.Images;
                    category.SeoInfo.MetaDescription = exception.SeoInfo?.MetaDescription;
                    category.SeoInfo.Title = exception.SeoInfo?.Title;
                }
                else
                {
                    category.Description = string.Empty;
                    category.Descriptions = new Model.EditorialReview[0];
                    category.Properties = new CatalogProperty[0];
                    category.Images = new Model.Image[0];
                    category.SeoInfo.MetaDescription = string.Empty;
                    category.SeoInfo.Title = string.Empty;
                }
            }
            var par = context.Parent;
            while (true && category.Images.Count == 0)
            {
                if (par == null)
                {
                    break;
                }
                if (par.Images.Count != 0)
                {
                    category.Images = par.Images;
                }
                par = par.Parent;
            }
            
        }

        protected virtual void CustomSeoCategory(ConverterContext context, Category category)
        {
            if (category.SeoInfo == null)
            {
                category.SeoInfo = new Model.SeoInfo();
            }
        }
    }
}