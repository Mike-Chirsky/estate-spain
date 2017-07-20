using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Catalog.Es
{
    public class CheckUrlRequest
    {
        public string Url { set; get; }
        public ProductFilterCriteria Terms { set; get; }
    }
}
