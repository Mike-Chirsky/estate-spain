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
            // Generate seo
            CustomSeoCategory(context, category);
            return category;
        }

        protected override void CustomSeoCategory(ConverterContext context, Category category)
        {
            if (category.SeoInfo == null)
            {
                category.SeoInfo = new Model.SeoInfo();
            }
            if (string.IsNullOrEmpty(category.SeoInfo.Title))
            {
                category.SeoInfo.Title = $"Недвижимость на {category.Name} купить недвижимость на {category.Name} недорого, цены в рублях";
            }

            if (string.IsNullOrEmpty(category.SeoInfo.MetaDescription))
            {
                category.SeoInfo.MetaDescription = $"&#127969; Недвижимость на {category.Name} – лучшие предложения от агентства Estate-Spain.com. &#9728; Продажа недвижимости на {category.Name} по низким ценам!" + " В нашем каталоге {0} {1}.";
            }
        }
    }
}