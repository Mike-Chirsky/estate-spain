using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Es;

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
            searchCriteria.RangeFilters = new Dictionary<string, Model.Common.NumericRange>();
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
                var term = GetRange(criteria.DisToSea);
                if (searchCriteria.Keyword == null)
                {
                    searchCriteria.Keyword = "";
                }
                else
                {
                    searchCriteria.Keyword += ",";
                }
                searchCriteria.Keyword = $"distancetosea:{term}";
                searchCriteria.RangeFilters.Add("distancetosea", term);
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
                searchCriteria.PriceRange = GetRange(criteria.Price);
                searchCriteria.RangeFilters.Add("price", searchCriteria.PriceRange);
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
                    Value = GetLocalizationValue(wc, criteria.Type, "other_type_dict")
                });
            }
            if (!string.IsNullOrEmpty(criteria.Cond))
            {
                terms.Add(new Term
                {
                    Name = "condition",
                    Value = GetLocalizationValue(wc, criteria.Cond, "condition")
                });
            }
            if (!string.IsNullOrEmpty(criteria.More))
            {
                var tersmValue = criteria.More.Trim(',').Split(',');
                foreach (var term in tersmValue)
                {
                    terms.Add(new Term
                    {
                        Name = "sys_filter",
                        Value = term.Trim()
                    });
                }
            }
            if (!string.IsNullOrEmpty(criteria.Tag))
            {
                terms.Add(new Term
                {
                    Name = "tag",
                    Value = GetLocalizationValue(wc, criteria.Tag, "tag")
                });
            }
            searchCriteria.Terms = terms.ToArray();
        }


        private static Model.Common.NumericRange GetRange(string value)
        {
            var range = new Model.Common.NumericRange
            {
                IncludeLower = true,
                IncludeUpper = true
            };
            if (string.IsNullOrEmpty(value))
                return range;
            if (value.Contains("low"))
            {
                decimal d;
                if (decimal.TryParse(value.Replace("low", ""), out d))
                {
                    range.Upper = d;
                }
            }
            else if (value.Contains("up"))
            {
                decimal d;
                if (decimal.TryParse(value.Replace("up", ""), out d))
                {
                    range.Lower = d;
                }
            }
            else if (value.Contains("-"))
            {
                var items = value.Replace(" ", "").Split('-');
                decimal d;
                if (decimal.TryParse(items[0], out d))
                {
                    range.Lower = d;
                }
                if (decimal.TryParse(items[1], out d))
                {
                    range.Upper = d;
                }
            }
            return range;
        }
        private static string GetLocalizationValue(WorkContext wc, string value, string key)
        {
            return wc.FilterSeoLinks[key]?.FirstOrDefault(x => x.Item2 == value)?.Item1;
        }

    }
}