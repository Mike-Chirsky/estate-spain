using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VirtoCommerce.LiquidThemeEngine.Converters.Extentsions;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Converters.Catalog;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Es;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Services.Es;

namespace VirtoCommerce.Storefront.Controllers
{
    [OutputCache(CacheProfile = "CatalogSearchCachingProfile")]
    public class CatalogSearchController : StorefrontControllerBase
    {
        private readonly ICatalogSearchService _searchService;
        private readonly ICategoryTreeService _categoryTreeService;
        public CatalogSearchController(WorkContext workContext, IStorefrontUrlBuilder urlBuilder, ICatalogSearchService searchService, ICategoryTreeService categoryTreeService)
            : base(workContext, urlBuilder)
        {
            _categoryTreeService = categoryTreeService;
            _searchService = searchService;
        }

        /// GET search
        /// This method used for search products by given criteria 
        /// <returns></returns>
        public ActionResult SearchProducts(string search)
        {
            //All resulting categories, products and aggregations will be lazy evaluated when view will be rendered. (workContext.Products, workContext.Categories etc) 
            //All data will loaded using by current search criteria taken from query string
            // map filter properies to search criteria
            WorkContext.CurrentProductSearchCriteria.Keyword = search;
            WorkContext.Search = search;
            var productFilterCriteria = new ProductFilterCriteria(WorkContext.QueryString);
            productFilterCriteria.FillTermsFromFileterCriteria(WorkContext.CurrentProductSearchCriteria, WorkContext);
            //For set filters value in UI
            WorkContext.CurrentCategory = new Category();
            
            return View("search", WorkContext);
        }

        /// <summary>
        /// GET search/{categoryId}?view=...
        /// This method called from SeoRoute when url contains slug for category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public async Task<ActionResult> CategoryBrowsing(string categoryId, string view)
        {
            categoryId = categoryId.Trim('/');
            var category = _categoryTreeService.FindByPath(categoryId);
            if (category == null)
            {
                category = (await _searchService.GetCategoriesAsync(new[] { categoryId }, CategoryResponseGroup.Full)).FirstOrDefault();
            }
            if (category == null)
            {
                throw new HttpException(404, String.Format("Category {0} not found.", categoryId));
            }

            WorkContext.CurrentCategory = category;
            WorkContext.CurrentPageSeo = category.SeoInfo.JsonClone();
            WorkContext.CurrentPageSeo.Slug = category.Url;
            // map filter properies to search criteria
            var productFilterCriteria = new ProductFilterCriteria(WorkContext.QueryString);
            productFilterCriteria.FillTermsFromFileterCriteria(WorkContext.CurrentProductSearchCriteria, WorkContext);
            var criteria = WorkContext.CurrentProductSearchCriteria.Clone();

            criteria.Outline = string.Format("{0}*", category.Outline); // should we simply take it from current category?
            category.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos) =>
            {
                criteria.PageNumber = pageNumber;
                criteria.PageSize = pageSize;
                if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                {
                    criteria.SortBy = SortInfo.ToString(sortInfos);
                }
                var result = _searchService.SearchProducts(criteria);
                //Prevent double api request for get aggregations
                //Because catalog search products returns also aggregations we can use it to populate workContext using C# closure
                //now workContext.Aggregation will be contains preloaded aggregations for current search criteria
                WorkContext.Aggregations = new MutablePagedList<Aggregation>(result.Aggregations);
                return result.Products;
            }, 1, ProductSearchCriteria.DefaultPageSize);


            // make sure title is set
            if (string.IsNullOrEmpty(WorkContext.CurrentPageSeo.Title))
            {
                WorkContext.CurrentPageSeo.Title = category.Name;
            }

            if (string.IsNullOrEmpty(view))
            {
                view = "grid";
            }

            if (view.Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                return View("collection.list", WorkContext);
            }
            return View("collection", WorkContext);
        }

        private void WriteToFile()
        {
            var dict = _categoryTreeService.GetSeoDict();
            using (var file = new System.IO.StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seo_dict.csv"), false, System.Text.Encoding.UTF8))
            {
                file.WriteLine("url;h1;description;title;seo text;region;city");
                var converter = new EsShopifyModelConverter();
                foreach (var key in dict.Keys)
                {
                    var liquidCategory = converter.ToLiquidCollection(dict[key], WorkContext);
                    var line = $"{key};\"{(string.IsNullOrEmpty(liquidCategory.H1) ? liquidCategory.FullName : liquidCategory.H1).Replace("\"", "\"\"")}\";\"{dict[key].SeoInfo.MetaDescription?.Replace("\"", "\"\"")}\";\"{dict[key].SeoInfo?.Title?.Replace("\"", "\"\"")}\";\"{liquidCategory.SeotextUp?.Replace("\"", "\"\"")}\";{liquidCategory.RegionName};{liquidCategory.CityName}";
                    file.WriteLine(line);
                }
            }
        }
    }
}
