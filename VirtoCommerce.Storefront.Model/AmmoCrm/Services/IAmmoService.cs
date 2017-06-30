using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.AmmoCrm.Services
{
    public interface IAmmoService
    {

        /// <summary>
        /// Auth in AmmoCrm
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="apiKey">User hash</param>
        /// <param name="subdomain">Sub domain in ammo serive</param>
        /// <returns></returns>
        Task<bool> Auth(string userName, string apiKey, string subdomain);

        /// <summary>
        /// Create leadm in AmmmoCrm
        /// </summary>
        /// <param name="leadModel"></param>
        /// <returns></returns>
        Task<bool> CreateUnsorted(AmmoUnsortedModel leadModel);
    }
}
