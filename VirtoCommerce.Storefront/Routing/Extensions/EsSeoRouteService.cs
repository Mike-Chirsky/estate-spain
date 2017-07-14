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
using VirtoCommerce.Storefront.Services.Es;

namespace VirtoCommerce.Storefront.Routing.Extensions
{
    public class EsSeoRouteService : SeoRouteService
    {
        private readonly Func<ICatalogModuleApiClient> _catalogApiFactory;
        private readonly ILocalCacheManager _cacheManager;
        private readonly ICategoryTreeService _categoryTreeService;

        public EsSeoRouteService(Func<ICoreModuleApiClient> coreApiFactory, Func<ICatalogModuleApiClient> catalogApiFactory, ILocalCacheManager cacheManager, ICategoryTreeService categoryTreeService) 
            : base(coreApiFactory, catalogApiFactory, cacheManager)
        {
            _categoryTreeService = categoryTreeService;
            _catalogApiFactory = catalogApiFactory;
            _cacheManager = cacheManager;
        }

        protected override SeoEntity FindEntityBySeoPath(string seoPath, WorkContext workContext)
        {
            if (_categoryTreeService.FindByPath(seoPath) == null)
            {
                var existSeo = base.FindEntityBySeoPath(seoPath, workContext);
                if (existSeo != null && (existSeo.ObjectType == "CatalogProduct" || existSeo.ObjectType == "Page" || existSeo.SeoPath.Equals("arenda", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return existSeo;
                }
                else
                {
                    return null;
                }
            }

            var pathParts = seoPath.Trim('/').Split('/');
            // TODO: Move fill terms to Tree service
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
                    workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                    {
                        Name = "region",
                        Value = product.Name
                    });
                }
                else if (product.CategoryId == ConfigurationManager.AppSettings["TypeCategoryId"])
                {
                    workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                    {
                        Name = "estatetype",
                        Value = product.Name
                    });
                }
                else if (product.CategoryId == ConfigurationManager.AppSettings["CityCategoryId"])
                {
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
                    }
                }
                else if (product.CategoryId == ConfigurationManager.AppSettings["TagCategoryId"])
                {
                    workContext.CurrentProductSearchCriteria.Terms = AddTerm(workContext.CurrentProductSearchCriteria.Terms, new Term
                    {
                        Name = "tags",
                        Value = product.Name
                    });
                }

                /*seo = all.FirstOrDefault(x => x.ObjectType == "Category");
                if (seo == null)
                {
                    return null;
                }
                existSeo = new SeoEntity { ObjectType = seo.ObjectType, ObjectId = seo.ObjectId, SeoPath = seoPath };*/
            }
            return new SeoEntity
            {
                ObjectType = "Category",
                ObjectId = seoPath,
                SeoPath = seoPath
            };
        }

        private Term[] AddTerm(Term[] terms, Term added)
        {
            var t = terms.ToList();
            t.Add(added);
            return t.ToArray();
        }
    }
}