using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Services.Es.Converters
{
    public class CityCategoryTreeConverter:DefaultCategoryTreeConverter
    {
        public override Category ToCategory(ConverterContext context, Product product)
        {
            var category = base.ToCategory(context, product);
            category.SeoPath = product.SeoInfo?.Slug;
            category.Url = product.SeoInfo?.Slug;
            category.Type = "city";
            category.RegionUrl = context.Parent?.SeoPath;
            category.CityUrl = product.SeoInfo?.Slug;
            category.RegionName = context.Parent?.Name;
            category.CityName = product.Name;
            return category;
        }
    }
}