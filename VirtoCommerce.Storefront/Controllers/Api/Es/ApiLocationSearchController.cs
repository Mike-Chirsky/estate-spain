using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VirtoCommerce.Storefront.Converters.Es;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Es.Search;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api.Es
{
    public class ApiLocationSearchController : StorefrontControllerBase
    {
        private readonly ICatalogSearchService _catalogSearchService;
        public ApiLocationSearchController(WorkContext context, IStorefrontUrlBuilder urlBuilder, ICatalogSearchService catalogSearchService) : base(context, urlBuilder)
        {
            _catalogSearchService = catalogSearchService;
        }

        // POST: /storefrontapi/location/search
        [HttpPost]
        public async Task<ActionResult> Search(string search)
        {
            WorkContext.CurrentStore = WorkContext.AllStores.FirstOrDefault(x => x.Id == ConfigurationManager.AppSettings["MasterStoreId"]);
            var foundProductsCity = await _catalogSearchService.SearchProductsAsync(new ProductSearchCriteria
            {
                Keyword = search,
                Outline = ConfigurationManager.AppSettings["CityCategoryId"],
                ResponseGroup = ItemResponseGroup.Seo | ItemResponseGroup.ItemAssociations
            });
            var foundProductsRegion = await _catalogSearchService.SearchProductsAsync(new ProductSearchCriteria
            {
                Keyword = search,
                Outline = ConfigurationManager.AppSettings["RegionCategoryId"],
                ResponseGroup = ItemResponseGroup.Seo
            });
            var products = foundProductsCity.Products.ToList();
            products.AddRange(foundProductsRegion.Products);
            products = products.OrderBy(x => x.Name).ToList();
            var result = new LocationSearchResult();
            result.Items.AddRange(products.Select(x => x.ToLocationItem()));
            //result.Items.AddRange(GetMockData());
            return Json(result);
        }

        private List<LocationSearchItem> GetMockData()
        {
            return new List<LocationSearchItem> {
              new LocationSearchItem  {
                  CityName = "Аликанте",
                  CitySeo = "alicante",
                  RegionName= "Коста дорада",
                  RegionSeo = "costa-dorada"
              },
              new LocationSearchItem{
                  RegionName ="Коста дорада",
                  RegionSeo = "costa-dorada"
              }
            };
        }
    }
}