using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Services.Es.Converters
{
    public class TypeCategoryTreeConverter: DefaultCategoryTreeConverter
    {
        public override Category ToCategory(ConverterContext context, Product product)
        {
            var category = base.ToCategory(context, product);
            category.Type = context.Parent.Type == null ? "type" : "type_add";
            if (!string.IsNullOrEmpty(context.Parent.Type))
            {
                category.RegionUrl = context.Parent.RegionUrl;
                category.CityUrl = context.Parent.CityUrl;
                category.RegionUrl = context.Parent.RegionName;
                category.CityUrl = context.Parent.CityName;
            }
            return category;
        }
    }
}