using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Routing.Extensions
{
    public class EsSeoRouteService : SeoRouteService
    {
        private readonly Func<ICatalogModuleApiClient> _catalogApiFactory;
        private readonly ILocalCacheManager _cacheManager;
        public EsSeoRouteService(Func<ICoreModuleApiClient> coreApiFactory, Func<ICatalogModuleApiClient> catalogApiFactory, ILocalCacheManager cacheManager) 
            : base(coreApiFactory, catalogApiFactory, cacheManager)
        {
            _catalogApiFactory = catalogApiFactory;
            _cacheManager = cacheManager;
        }

        protected override SeoEntity FindEntityBySeoPath(string seoPath, WorkContext workContext)
        {
            var existSeo = base.FindEntityBySeoPath(seoPath, workContext);
            if (existSeo != null && existSeo.ObjectType == "CatalogProduct")
            {
                return existSeo;
            }
            
            var pathParts = seoPath.Split('/');

            foreach (var part in pathParts)
            {
                var all = GetAllSeoRecords(part);
                var seo = all.FirstOrDefault(x => x.ObjectType == "CatalogProduct");
                if (seo == null)
                {
                    return null;
                }
                var product = _cacheManager.Get($"Product{seo.ObjectId}", "SeoProducts", () => _catalogApiFactory()
                                                                                                .CatalogModuleProducts.GetProductById(seo.ObjectId)
                                                                                                ?.ToProduct(workContext.CurrentLanguage,
                                                                                                            workContext.CurrentCurrency,
                                                                                                            workContext.CurrentStore));
                if (product == null)
                {
                    return null;
                }
                // TODO: move id to settings
                switch (product.CategoryId)
                {
                    case "654dbd245eb2484fa5ccd8ffd69387da":
                        workContext.RegionProduct = product;
                        workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                        {
                            Name = "region",
                            Value = product.Name
                        });
                        break;
                    case "2036643b08794d149e1722adbe0230e8":
                        workContext.TypeProduct = product;
                        workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                        {
                            Name = "estatetype",
                            Value = product.Name
                        });
                        break;
                    case "06d09840154341a4b4564aa9b94b06b3":
                        workContext.CityProduct = product;
                        workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                        {
                            Name = "city",
                            Value = product.Name
                        });
                        break;
                }
                seo = all.FirstOrDefault(x => x.ObjectType == "Category");
                if (seo == null)
                {
                    return null;
                }
                existSeo = new SeoEntity { ObjectType = seo.ObjectType, ObjectId = seo.ObjectId, SeoPath = seoPath };
            }
            return existSeo;
        }

        private Term[] AddTerm(Term[] terms, Term added)
        {
            var t = terms.ToList();
            t.Add(added);
            return t.ToArray();
        }
    }
}