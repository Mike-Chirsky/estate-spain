using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.LiquidThemeEngine.Converters.Extentsions
{
    public partial class EsShopifyModelConverter
    {
        public override Collection ToLiquidCollection(Category category, WorkContext workContext)
        {
            var collection = base.ToLiquidCollection(category, workContext);
            collection.CurrentTagCollection = new TagCollection(
                        workContext.CurrentProductSearchCriteria.Terms.Select(t => ToLiquidTag(t)).ToList());
            return collection;
        }
    }
}
