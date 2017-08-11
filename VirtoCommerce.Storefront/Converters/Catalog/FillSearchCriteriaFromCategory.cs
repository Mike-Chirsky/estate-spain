using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Services.Es;

namespace VirtoCommerce.Storefront.Converters.Catalog
{
    public static class FillSearchCriteriaFromCategory
    {
        public static void FillSearchCriteriaTerms(this Category category, ProductSearchCriteria searchCriteria)
        {
            var terms = searchCriteria.Terms.ToList();
            var parent = category;
            do {
                FillTerms(parent, terms);
                if (parent.ProductType == ESCategoryTreeService.CitiesKey)
                {
                    break;
                }
                parent = parent.Parent;
            }
            while (parent != null || parent.Parent != null);
            terms.Add(new Term
            {
                Name = "available",
                Value = "Неизвестно,Доступно"
            });
            terms.Add(new Term
            {
                Name = "dealtype",
                Value = "Продажа"
            });
            searchCriteria.Terms = terms.ToArray();
        }

        private static void FillTerms(Category category, List<Term> terms)
        {
            if (category.ProductType == ESCategoryTreeService.RegionKey)
            {
                terms.Add(new Term
                {
                    Name = "region",
                    Value = category.Name
                });
            }
            else if (category.ProductType == ESCategoryTreeService.EstateTypeKey)
            {
                terms.Add(new Term
                {
                    Name = "estatetype",
                    Value = category.Name
                });
            }
            else if (category.ProductType == ESCategoryTreeService.CitiesKey)
            {
                terms.Add(new Term
                {
                    Name = "city",
                    Value = category.Name
                });
            }
            else if (category.ProductType == ESCategoryTreeService.TagsKey)
            {
                terms.Add(new Term
                {
                    Name = "tag",
                    Value = category.Name
                });
            }
            else if (category.ProductType == ESCategoryTreeService.OtherTypeKey)
            {
                terms.Add(new Term
                {
                    Name = "other_type",
                    Value = category.Name
                });
            }
            else if (category.ProductType == ESCategoryTreeService.ConditionKey)
            {
                terms.Add(new Term
                {
                    Name = "condition",
                    Value = category.Name
                });
            }
            else if (category.ProductType == ESCategoryTreeService.SinglePageKey)
            {
                terms.Add(new Term
                {
                    Name = category.Properties.FirstOrDefault(x => x.Name == "term-name")?.Value,
                    Value = category.Properties.FirstOrDefault(x => x.Name == "term-value")?.Value
                });
            }
        }
    }
}