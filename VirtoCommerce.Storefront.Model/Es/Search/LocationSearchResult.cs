using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Es.Search
{
    public class LocationSearchResult
    {
        public LocationSearchResult()
        {
            Items = new List<LocationSearchItem>();
        }
        public IList<LocationSearchItem> Items { set; get; }
    }
}
