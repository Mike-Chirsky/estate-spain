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
                result.SearchText = workContext.Search;
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
            KeyValue<string, string> region = null;
            if (workContext.FilterSeoLinks.ContainsKey("region"))
            {
                region = AddBreadcrumb(product.Breadcrumb, workContext.FilterSeoLinks["region"].FirstOrDefault(x => x.Item1 == product.Region), workContext);
            }
            KeyValue<string, string> city = null;
            if (workContext.FilterSeoLinks.ContainsKey("city"))
            {
                city = AddBreadcrumb(product.Breadcrumb, workContext.FilterSeoLinks["city"].FirstOrDefault(x => x.Item1 == product.City), workContext);
            }
            if (workContext.FilterSeoLinks.ContainsKey("estatetype"))
            {
                AddBreadcrumb(product.Breadcrumb, workContext.FilterSeoLinks["estatetype"].FirstOrDefault(x => x.Item1 == product.Estatetype), workContext, city?.Value ?? region?.Value ?? "");
            }
            product.Breadcrumb.Add(new KeyValue<string, string>(product.BreadcrumbTitle, ""));
        }

        private KeyValue<string, string> AddBreadcrumb(ICollection<KeyValue<string, string>> breadcrumb, Tuple<string, string> existProp, WorkContext wc, string parentUrl = "")
        {
            if (existProp != null)
            {
                var url = $"{parentUrl}/{existProp.Item2}".Trim('/');
                if (!wc.CategoryRoutes.Keys.Contains(url))
                {
                    return null;
                }
                var add = new KeyValue<string, string>(existProp.Item1, url);
                breadcrumb.Add(add);
                return add;
            }
            return null;
        }
    }
}
