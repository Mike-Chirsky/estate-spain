using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Services.Es.Converters
{
    public class ConditionCategoryTreeConverter: DefaultCategoryTreeConverter
    {
        public override Category ToCategory(ConverterContext context, Product product)
        {
            var category = base.ToCategory(context, product);
            if (!string.IsNullOrEmpty(context.Parent.Type))
            {
                category.Type = "condition_add";
                // TODO: For not display info from main category
                category.Properties = new CatalogProperty[0];
                category.Description = string.Empty;
                category.Descriptions = new Model.EditorialReview[0];
                category.RegionUrl = context.Parent.RegionUrl;
                category.CityUrl = context.Parent.CityUrl;
                category.RegionName = context.Parent.RegionName;
                category.CityName = context.Parent.CityName;
            }
            else
            {
                category.Type = "condition";
            }
            return category;
        }

        protected override string CreateFullName(ConverterContext context, Product product)
        {
            if (context.Parent?.Parent?.Id != null)
                return $"{context.Parent?.Name} в {product.Name} в {context.Parent?.Parent?.Name}";
            if (context.Parent?.Parent?.Id == null)
                return $"{product.Name} в {context.Parent?.Name}";
            return product.Name;
        }
    }
}