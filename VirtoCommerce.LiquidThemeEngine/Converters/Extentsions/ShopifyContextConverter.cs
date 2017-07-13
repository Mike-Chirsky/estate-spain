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
            if (result.Product != null)
            {
                SetBreadcrumbToProduct(result.Product, workContext);
            }
            return result;
        }

        private void SetBreadcrumbToProduct(Product product, WorkContext workContext)
        {
            product.Breadcrumb = new List<KeyValue<string, string>>();
            var region = AddBreadcrumb(product.Breadcrumb, workContext.FilterSeoLinks["region"].FirstOrDefault(x => x.Item1 == product.Region));
            var city = AddBreadcrumb(product.Breadcrumb, workContext.FilterSeoLinks["city"].FirstOrDefault(x => x.Item1 == product.City));
            AddBreadcrumb(product.Breadcrumb, workContext.FilterSeoLinks["estatetype"].FirstOrDefault(x => x.Item1 == product.Estatetype), city.Value);
            product.Breadcrumb.Add(new KeyValue<string, string>(product.BreadcrumbTitle, ""));
        }

        private KeyValue<string, string> AddBreadcrumb(ICollection<KeyValue<string, string>> breadcrumb, Tuple<string, string> existProp, string parentUrl = "")
        {
            if (existProp != null)
            {
                var add = new KeyValue<string, string>(existProp.Item1, $"{parentUrl}/{existProp.Item2}");
                breadcrumb.Add(add);
                return add;
            }
            return null;
        }
    }
}
