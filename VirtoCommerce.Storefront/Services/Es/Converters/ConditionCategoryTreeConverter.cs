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
                if (!FillFromException(context, category))
                {
                    return null;
                }
                category.RegionUrl = context.Parent.RegionUrl;
                category.CityUrl = context.Parent.CityUrl;
                category.RegionName = context.Parent.RegionName;
                category.CityName = context.Parent.CityName;
            }
            else
            {
                category.Type = "condition";
            }
            // Generate seo
            CustomSeoCategory(context, category);
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

        protected override void CustomSeoCategory(ConverterContext context, Category category)
        {
            if (category.SeoInfo == null)
            {
                category.SeoInfo = new Model.SeoInfo();
            }
            var ptext = category.Type == "city" ? "в" : "на";
            if (string.IsNullOrEmpty(category.SeoInfo.Title))
            {
                if (string.IsNullOrEmpty(category.Parent.Type))
                {
                    category.SeoInfo.Title = $"Недвижимость в {category.Name} – купить недвижимость в {category.Name} недорого, цены в рублях";
                }
                else
                {
                    category.SeoInfo.Title = $"Недвижимость {ptext} {category.Parent.Name} – купить {category.Name} {ptext} {category.Parent.Name} недорого, цены в рублях";
                }

                if (string.IsNullOrEmpty(category.SeoInfo.MetaDescription))
                {
                    if (string.IsNullOrEmpty(category.Parent.Type))
                    {
                        category.SeoInfo.MetaDescription = $"&#127969; Недвижимость в {category.Name} – лучшие предложения от агентства Estate-Spain.com &#9728; Продажа недвижимости по низким ценам!" + " В нашем каталоге представленно {0}.";
                    }
                    else
                    {
                        category.SeoInfo.MetaDescription = $"&#127969; Недвижимость {ptext} {category.Parent.Name} – лучшие предложения от агентства Estate-Spain.com &#9728; Продажа недвижимости по низким ценам!" + " В нашем каталоге представленно {0}.";
                    }
                }
            }
        }
    }
}