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

        protected override string CreateFullName(ConverterContext context, Product product)
        {
            return $"Недвижимость в {product.Name}";
        }

        protected override void CustomSeoCategory(ConverterContext context, Category category)
        {
            if (category.SeoInfo == null)
            {
                category.SeoInfo = new Model.SeoInfo();
            }
            if (string.IsNullOrEmpty(category.SeoInfo.Title))
            {
                category.SeoInfo.Title = $"Недвижимость в {category.Name} купить недвижимость в {category.Name} недорого, цены в рублях";
            }

            if (string.IsNullOrEmpty(category.SeoInfo.MetaDescription))
            {
                category.SeoInfo.MetaDescription = $"Недвижимость в {category.Name} – лучшие предложения от агентства Estate-Spain.com. Продажа недвижимости в {category.Name} по низким ценам!" + " В нашем каталоге представлено {0}.";
            }
        }
    }
}