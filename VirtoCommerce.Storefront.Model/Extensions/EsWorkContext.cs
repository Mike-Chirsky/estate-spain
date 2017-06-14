using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model
{
    public partial class WorkContext
    {
        /// <summary>
        /// Info for seo Region
        /// </summary>
        public Product RegionProduct { set; get; }

        /// <summary>
        /// Info for seo City
        /// </summary>
        public Product CityProduct { set; get; }

        /// <summary>
        /// Info for type product
        /// </summary>
        public Product TypeProduct { set; get; }
    }
}
