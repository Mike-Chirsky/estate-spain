using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Services.Es.Converters
{
    public class OtherTypeCategoryTreeConverter: DefaultCategoryTreeConverter
    {
        public override Category ToCategory(ConverterContext context, Product product)
        {
            var category = base.ToCategory(context, product);
            if (!string.IsNullOrEmpty(context.Parent.Type))
            {
                category.Type = "other_type_add";
                FillFromException(context, category);
                category.RegionUrl = context.Parent.RegionUrl;
                category.CityUrl = context.Parent.CityUrl;
                category.RegionName = context.Parent.RegionName;
                category.CityName = context.Parent.CityName;
            }
            else
            {
                category.Type = "other_type";
            }
            // Generate seo
            CustomSeoCategory(context, category);
            return category;
        }

        protected override void CustomSeoCategory(ConverterContext context, Category category)
        {
            base.CustomSeoCategory(context, category);
            if (string.IsNullOrEmpty(category.SeoInfo.Title))
            {
                category.SeoInfo.Title = $"{category.Name} – купить {category.Name} недорого, цены в рублях";
            }
            if (string.IsNullOrEmpty(category.SeoInfo.MetaDescription))
            {
                category.SeoInfo.MetaDescription = $"&#127969; {category.Name} – лучшие предложения от агентства Estate-Spain.com &#9728; Продажа недвижимости по низким ценам!" + " В нашем каталоге представлено {0}";
            }
        }
    }
}