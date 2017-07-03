using System;
using System.Collections.Generic;
using System.Configuration;
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
            if (existSeo != null && (existSeo.ObjectType == "CatalogProduct" || existSeo.ObjectType == "Page" || existSeo.SeoPath.Equals("arenda", StringComparison.InvariantCultureIgnoreCase)))
            {
                return existSeo;
            }
            
            var pathParts = seoPath.Trim('/').Split('/');

            foreach (var part in pathParts)
            {
                var all = GetAllSeoRecords(part);
                var seo = all.FirstOrDefault(x => x.ObjectType == "CatalogProduct");
                if (seo == null)
                {
                    return null;
                }
                var product = _cacheManager.Get($"Product{seo.ObjectId}", "SeoProducts", () => _catalogApiFactory()
                                                                                                .CatalogModuleProducts.GetProductById(seo.ObjectId, ItemResponseGroup.ItemMedium.ToString())
                                                                                                ?.ToProduct(workContext.CurrentLanguage,
                                                                                                            workContext.CurrentCurrency,
                                                                                                            workContext.CurrentStore));
                if (product == null)
                {
                    return null;
                }
                if (product.CategoryId == ConfigurationManager.AppSettings["RegionCategoryId"])
                {
                    workContext.RegionProduct = product;
                    workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                    {
                        Name = "region",
                        Value = product.Name
                    });
                }
                else if (product.CategoryId == ConfigurationManager.AppSettings["TypeCategoryId"])
                {
                    workContext.TypeProduct = product;
                    workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                    {
                        Name = "estatetype",
                        Value = product.Name
                    });
                }
                else if (product.CategoryId == ConfigurationManager.AppSettings["CityCategoryId"])
                {
                    workContext.CityProduct = product;
                    workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                    {
                        Name = "city",
                        Value = product.Name
                    });
                    // link to region
                    var regionAssociationId = product.Associations.OfType<ProductAssociation>().FirstOrDefault(x => x.Type == "Regions")?.ProductId;
                    if (!string.IsNullOrEmpty(regionAssociationId))
                    {
                        var regionProduct = _cacheManager.Get($"Product{regionAssociationId}", "SeoProducts", () => _catalogApiFactory()
                                                                                                .CatalogModuleProducts.GetProductById(regionAssociationId, ItemResponseGroup.ItemAssociations.ToString())
                                                                                                ?.ToProduct(workContext.CurrentLanguage,
                                                                                                            workContext.CurrentCurrency,
                                                                                                            workContext.CurrentStore));
                        workContext.RegionProduct = regionProduct;
                    }
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