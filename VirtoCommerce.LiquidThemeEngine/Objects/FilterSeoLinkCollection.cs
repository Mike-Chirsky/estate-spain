using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class FilterSeoLinkCollection : ItemCollection<FilterLink>
    {
        public string Key { set; get; }
        public FilterSeoLinkCollection(IEnumerable<FilterLink> superset, string key) : base(superset)
        {
            Key = key;
        }
    }
}
