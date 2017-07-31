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
            if (!string.IsNullOrEmpty(context.Parent.Type))
            {
                category.Type = "type_add";
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
                category.Type = "type";
            }
            // Generate seo
            CustomSeoCategory(context, category);
            return category;
        }

        protected override string CreateFullName(ConverterContext context, Product product)
        {
            if (context.Parent.Id != null)
                return $"{product.Name} в {context.Parent.Name}";
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
                if (category.Parent != null && !string.IsNullOrEmpty(category.Parent.Id))
                {
                    var index = category.Name.IndexOf(" и ");
                    var names = new string[] { "", "" };
                    names[0] = category.Name.Substring(0, index == -1 ? category.Name.Length : index);
                    if (index != -1)
                    {
                        names[1] = category.Name.Substring(index + 3);
                    }
                    category.SeoInfo.Title = $"{category.Name} {ptext} {category.Parent.Name} - купить {string.Join(" или ", names.Select(x => x.ToLower().Trim()))} {ptext} {category.Parent.Name} недорого, цены в рублях";

                }
                else
                {
                    category.SeoInfo.Title = $"{category.Name} в Испании";
                }
            }

            if (string.IsNullOrEmpty(category.SeoInfo.MetaDescription))
            {
                if (!string.IsNullOrEmpty(category.Parent.Id))
                {
                    category.SeoInfo.MetaDescription = $"&#127969; {category.Name} {ptext} {category.Parent.Name} – лучшие предложения от агентства Estate-Spain.com. &#9728; Продажа жилья {ptext} {category.Parent.Name} по низким ценам! " + "В нашем каталоге представленно {0}.";
                }
                else
                {
                    category.SeoInfo.MetaDescription = $"&#127969; {category.Name} – лучшие предложения от агентства Estate-Spain.com. &#9728; Продажа жилья по низким ценам! " + "В нашем каталоге представленно {0}.";
                }
            }
        }
    }
}