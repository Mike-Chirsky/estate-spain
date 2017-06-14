using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class ShopifyThemeWorkContext
    {
        public Product RegionProduct { set; get; }
        public Product CityProduct { set; get; }
        public Product TypeProduct { set; get; }
    }
}
