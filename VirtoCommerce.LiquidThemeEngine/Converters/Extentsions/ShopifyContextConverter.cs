using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Converters.Extentsions
{
    public partial class EsShopifyModelConverter
    {
        public override ShopifyThemeWorkContext ToLiquidContext(WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var result = base.ToLiquidContext(workContext, urlBuilder);
            if (workContext.QueryString.AllKeys.FirstOrDefault(x => x.Equals("from_filter", StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                result.FromFilter = true;
            }
            if (workContext.FilterSeoLinks != null)
            {
                result.FilterSeoLinks = FilterSoeLinksConverter.ToSeoLinksCollections(workContext.FilterSeoLinks);
            }
            if (workContext.CurrentProductSearchCriteria != null && !string.IsNullOrEmpty(workContext.CurrentProductSearchCriteria.Keyword))
            {
                result.Keyword = workContext.CurrentProductSearchCriteria.Keyword;
            }
            return result;
        }
    }
}
