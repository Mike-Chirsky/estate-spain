using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Services.Es
{
    public interface IResetCacheProductService
    {
        void ResetProductCache(string path, WorkContext workContext);
             
    }
}
