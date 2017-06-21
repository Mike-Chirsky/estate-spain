using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class FilterSeoLinkCollections : ItemCollection<FilterSeoLinkCollection>
    {
        public FilterSeoLinkCollections(IEnumerable<FilterSeoLinkCollection> superset) : base(superset)
        {
        }
    }
}
