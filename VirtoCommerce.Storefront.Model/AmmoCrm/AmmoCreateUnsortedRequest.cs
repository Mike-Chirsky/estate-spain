using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.AmmoCrm
{
    public class AmmoCreateUnsortedRequest
    {
        public AmmoCreateUnsortedRequest()
        {
            Add = new List<Dictionary<string, object>>();
        }
        public string Category { set; get; }
        public List<Dictionary<string, object>> Add { set; get; }
    }
}
