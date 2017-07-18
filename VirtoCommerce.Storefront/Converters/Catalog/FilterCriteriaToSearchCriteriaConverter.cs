using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Extensions;

namespace VirtoCommerce.Storefront.Converters.Catalog
{
    public static class FilterCriteriaToSearchCriteriaConverter
    {
        public static void FillTermsFromFileterCriteria(this ProductFilterCriteria criteria, ProductSearchCriteria searchCriteria, WorkContext wc)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException($"{nameof(criteria)}");
            }
            if (searchCriteria == null)
            {
                throw new ArgumentNullException($"{nameof(searchCriteria)}");
            }
            if (wc == null)
            {
                throw new ArgumentNullException($"{nameof(wc)}");
            }
            var terms = searchCriteria.Terms.ToList();
            if (!string.IsNullOrEmpty(criteria.Bath))
            {
                terms.Add(new Term
                {
                    Name = "bath",
                    Value = criteria.Bath
                });
            }
            if (!string.IsNullOrEmpty(criteria.Broom))
            {
                terms.Add(new Term
                {
                    Name = "bedrooms",
                    Value = criteria.Broom
                });
            }
            if (!string.IsNullOrEmpty(criteria.City))
            {
                terms.Add(new Term
                {
                    Name = "city",
                    Value = GetLocalizationValue(wc, criteria.City, "city")
                });
            }
            if (!string.IsNullOrEmpty(criteria.DisToSea))
            {
                terms.Add(new Term
                {
                    Name = "distancetosea",
                    Value = criteria.DisToSea
                });
            }
            if (!string.IsNullOrEmpty(criteria.EstateType))
            {
                terms.Add(new Term
                {
                    Name = "estatetype",
                    Value = GetLocalizationValue(wc, criteria.EstateType, "estatetype")
                });
            }
            if (!string.IsNullOrEmpty(criteria.Ls))
            {
                terms.Add(new Term
                {
                    Name = "landsquare",
                    Value = criteria.Ls
                });
            }
            if (!string.IsNullOrEmpty(criteria.Price))
            {
                terms.Add(new Term
                {
                    Name = "price",
                    Value = criteria.Price
                });
            }
            if (!string.IsNullOrEmpty(criteria.Region))
            {
                terms.Add(new Term
                {
                    Name = "region",
                    Value = GetLocalizationValue(wc, criteria.Region, "region")
                });
            }
            if (!string.IsNullOrEmpty(criteria.Sq))
            {
                terms.Add(new Term
                {
                    Name = "propertysquare",
                    Value = criteria.Sq
                });
            }
            if (!string.IsNullOrEmpty(criteria.Type))
            {
                terms.Add(new Term
                {
                    Name = "other_type",
                    Value = GetLocalizationValue(wc, criteria.Type, "other_type")
                });
            }
            if (!string.IsNullOrEmpty(criteria.Condition))
            {
                terms.Add(new Term
                {
                    Name = "condition",
                    Value = GetLocalizationValue(wc, criteria.Type, "condition")
                });
            }
            if (!string.IsNullOrEmpty(criteria.More))
            {
                terms.Add(new Term
                {
                    Name = "sys_filter",
                    Value = criteria.More
                });
            }
            searchCriteria.Terms = terms.ToArray();
        }

        public static string GetLocalizationValue(WorkContext wc, string value, string key)
        {
            return wc.FilterSeoLinks[key]?.FirstOrDefault(x => x.Item2 == value)?.Item1;
        }

    }
}