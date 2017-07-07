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

        public Product MetaInfoProduct
        {
            get
            {
                if (TypeProduct != null)
                {
                    return TypeProduct;
                }
                if (CityProduct != null)
                {
                    return CityProduct;
                }
                return RegionProduct;
            }
        }

        public string CollectionType
        {
            get
            {
                if (TypeProduct != null)
                {
                    return "type";
                }
                if (CityProduct != null)
                {
                    return "city";
                }
                if (RegionProduct != null)
                {
                    return "region";
                }
                return null;
            }
        }

        public bool FromFilter { set; get; }

        public string Keyword { set; get; }

        public FilterSeoLinkCollections FilterSeoLinks { set; get; }
    }
}
