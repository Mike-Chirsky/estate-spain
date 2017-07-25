using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class ShopifyThemeWorkContext
    {
        public bool FromFilter { set; get; }

        public string SearchText { set; get; }

        public FilterSeoLinkCollections FilterSeoLinks { set; get; }
    }
}
