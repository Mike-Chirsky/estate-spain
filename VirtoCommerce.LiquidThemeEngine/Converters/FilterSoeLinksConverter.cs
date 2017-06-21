using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine.Objects;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class FilterSoeLinksConverter
    {
        public static FilterSeoLinkCollections ToSeoLinksCollections(Dictionary<string, List<Tuple<string, string>>> seoLinks)
        {
            var collection = new List<FilterSeoLinkCollection>();
            foreach (var key in seoLinks.Keys)
            {
                collection.Add(ToSeoLinskCollenction(key, seoLinks[key]));
            }
            return new FilterSeoLinkCollections(collection);
        }

        public static FilterSeoLinkCollection ToSeoLinskCollenction(string key, List<Tuple<string, string>> values)
        {
            return new FilterSeoLinkCollection(values.Select(x => new FilterLink { ValueFilter = x.Item1, SeoLink = x.Item2 }), key);
        }
    }
}
