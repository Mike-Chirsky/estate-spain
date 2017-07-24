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
        /// List filter seo keywords
        /// </summary>
        public Dictionary<string, List<Tuple<string, string>>> FilterSeoLinks { get; set; }

        public string Search { set; get; }
    }
}
