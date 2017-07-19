using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Services.Es.Converters
{
    public class TagCategoryTreeConverter: DefaultCategoryTreeConverter
    {
        public override Category ToCategory(ConverterContext context, Product product)
        {
            var category = base.ToCategory(context, product);
            category.Type = "tag";
            if (!string.IsNullOrEmpty(context.Parent.Type))
            {
                category.Type = "tag_add";
                FillFromException(context, category);
                category.RegionUrl = context.Parent.RegionUrl;
                category.CityUrl = context.Parent.CityUrl;
                category.RegionName = context.Parent.RegionName;
                category.CityName = context.Parent.CityName;
            }
            else
            {
                category.Type = "tag";
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
                if (string.IsNullOrEmpty(category.Parent.Type))
                {
                    category.SeoInfo.Title = $"Недвижимость {category.Name} – купить недвижимость {category.Name} недорого, цены в рублях";
                }
                else
                {

                    category.SeoInfo.Title = $"{category.Parent.FullName} {category.Name} – купить {category.Parent.Name} {category.Name} недорого, цены в рублях";
                }
            }
            if (string.IsNullOrEmpty(category.SeoInfo.MetaDescription))
            {
                if (string.IsNullOrEmpty(category.Parent.Type))
                {
                    category.SeoInfo.MetaDescription = $"&#127969; Недвижимость {category.Name} – лучшие предложения от агентства Estate-Spain.com &#9728; Продажа недвижимости по низким ценам!" + " В нашем каталоге представлено {0}";
                }
                else
                {
                    category.SeoInfo.MetaDescription = $"&#127969; {category.Parent.FullName} {category.Name} – лучшие предложения от агентства Estate-Spain.com &#9728; Продажа недвижимости по низким ценам!" + " В нашем каталоге представлено {0}";
                }
            }
        }
    }
}