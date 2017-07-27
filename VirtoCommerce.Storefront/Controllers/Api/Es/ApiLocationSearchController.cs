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
            var oldCurrentStore = WorkContext.CurrentStore;
            WorkContext.CurrentStore = WorkContext.AllStores.FirstOrDefault(x => x.Id == ConfigurationManager.AppSettings["MasterStoreId"]);
            var products = (await _catalogSearchService.SearchProductsAsync(new ProductSearchCriteria
            {
                Keyword = search,
                Outlines = new[] { ConfigurationManager.AppSettings["CityCategoryId"], ConfigurationManager.AppSettings["RegionCategoryId"] },
                ResponseGroup = ItemResponseGroup.Seo | ItemResponseGroup.ItemAssociations,
                AssociationsResponseGroup = ItemResponseGroup.ItemSmall,
            }))?.Products.OrderBy(x => x.Name).ToList();
            WorkContext.CurrentStore = oldCurrentStore;
            // load associations 
            var associations = await _catalogSearchService.GetProductsAsync(products.SelectMany(x => x.Associations.OfType<ProductAssociation>().Select(s => s.ProductId)).ToArray(), ItemResponseGroup.ItemSmall);
            foreach (var association in products.SelectMany(x => x.Associations.OfType<ProductAssociation>()))
            {
                var foundAssocaitionProd = associations.FirstOrDefault(x => x.Id == association.ProductId);
                if (foundAssocaitionProd != null)
                {
                    association.Product = foundAssocaitionProd;
                }
            }
            var result = new LocationSearchResult();
            result.Items.AddRange(products.Select(x => x.ToLocationItem()));
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