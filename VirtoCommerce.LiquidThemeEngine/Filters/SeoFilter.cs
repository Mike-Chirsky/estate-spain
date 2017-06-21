using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine.Objects;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public class SeoFilter
    {
        /// <summary>
        /// Filtering soe links for filter
        /// </summary>
        /// <param name="input">Object name</param>
        /// <param name="type">Type filter (estatetype, other_type and etc)</param>
        /// <param name="filters">Dictionary filters</param>
        /// <returns>Get seo link</returns>
        public static string FilterSeoLink(string input, string type, object filters)
        {
            var convertedFilters = filters as FilterSeoLinkCollections;
            if (convertedFilters == null)
                return null;
            var collection = convertedFilters.FirstOrDefault(x => x.Key == type);
            if (collection != null)
            {
                return collection.FirstOrDefault(x => x.ValueFilter == input)?.SeoLink;
            }
            return null;
        }

        /// <summary>
        /// Get collection filter types
        /// </summary>
        /// <param name="type">Type filter</param>
        /// <param name="input">Dictionary filters</param>
        /// <returns>List of type seo links (estatetype, other_type and etc)</returns>
        public static object ListSeoLinkType(object input, string type)
        {
            var convertedFilters = input as FilterSeoLinkCollections;
            if (convertedFilters == null)
                return null;
            return convertedFilters.FirstOrDefault(x => x.Key == type);
        }
    }
}
