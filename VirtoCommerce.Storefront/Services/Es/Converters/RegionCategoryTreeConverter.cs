using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Services.Es.Converters
{
    public class RegionCategoryTreeConverter: DefaultCategoryTreeConverter
    {
        public override Category ToCategory(ConverterContext context, Product product)
        {
            var category = base.ToCategory(context, product);
            category.Type = "region";
            category.RegionUrl = product.SeoInfo?.Slug;
            category.RegionName = product.Name;
            return category;
        }
    }
}