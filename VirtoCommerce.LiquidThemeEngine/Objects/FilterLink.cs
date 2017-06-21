using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    /// <summary>
    /// Use for display seo link for filter
    /// </summary>
    public class FilterLink : Drop
    {
        public string ValueFilter { set; get; }
        public string SeoLink { set; get; }
    }
}

