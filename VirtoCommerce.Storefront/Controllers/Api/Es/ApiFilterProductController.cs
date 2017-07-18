using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VirtoCommerce.Storefront.Converters.Catalog;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Extensions;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api.Es
{
    public class ApiFilterProductController : StorefrontControllerBase
    {
        private readonly ICatalogSearchService _catalogSearchService;

        public ApiFilterProductController(WorkContext context, IStorefrontUrlBuilder urlBuilder, ICatalogSearchService catalogSearchService) : base(context, urlBuilder)
        {
            _catalogSearchService = catalogSearchService;
        }

        // storefrontapi/product/filter
        [HttpPost]
        public async Task<ActionResult> FilterProducts(ProductFilterCriteria filterCriteria)
        {
            var searchCriteria = new ProductSearchCriteria(WorkContext.CurrentLanguage, WorkContext.CurrentCurrency, WorkContext.QueryString);
            searchCriteria.ResponseGroup = ItemResponseGroup.ItemInfo;
            filterCriteria.FillTermsFromFileterCriteria(searchCriteria, WorkContext);
            var retVal = await _catalogSearchService.SearchProductsAsync(searchCriteria);
            return Json(new
            {
                Aggregations = retVal.Aggregations,
                MetaData = retVal.Products.GetMetaData()
            });
        }
    }
}