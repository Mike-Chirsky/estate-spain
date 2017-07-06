using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api.Es
{
    public class ApiMarketController : StorefrontControllerBase
    {
        private readonly ICatalogSearchService _catalogSearchService;

        public ApiMarketController(WorkContext context, IStorefrontUrlBuilder urlBuilder, ICatalogSearchService catalogSearchService) : base(context, urlBuilder)
        {
            _catalogSearchService = catalogSearchService;
        }

        // GET: storefrontapi/market/{type}/{id}/{page}/{pageSize}
        [HttpGet]
        public ActionResult GetMarketPage(string type, string id, int page, int pageSize)
        {
            WorkContext.CurrentProductSearchCriteria.PageNumber = page;
            WorkContext.CurrentProductSearchCriteria.PageSize = pageSize;
            type = type.ToLower();
            switch (type)
            {
                case "region":
                case "city":
                case "type":
                case "main":
                    SearchProduct(type, id, page, pageSize);
                    return View("market-block/market-block-partial-product", "empty", WorkContext);
                case "blog":
                    SearchBlog(id, page, pageSize);
                    return View("market-block/market-block-partial-blog", "empty", WorkContext);
                default:
                    return null;
            }
        
        }

        private void SearchBlog(string id, int page, int pageSize)
        {

        }

        private void SearchProduct(string type, string id, int page, int pageSize)
        {
            switch (type)
            {
                case "region":
                    WorkContext.CurrentProductSearchCriteria.MutableTerms = new List<Term> {
                        new Term
                        {
                            Name = "region",
                            Value = id
                        }
                    };
                    break;
                case "city":
                    WorkContext.CurrentProductSearchCriteria.MutableTerms = new List<Term> {
                        new Term
                        {
                            Name = "city",
                            Value = id
                        }
                    };
                    break;
                case "type":
                    WorkContext.CurrentProductSearchCriteria.MutableTerms = new List<Term> {
                        new Term
                        {
                            Name = "estatetype",
                            Value = id
                        }
                    };
                    break;
            }
            WorkContext.Products = new MutablePagedList<Product>((pageNumber, pSize, sortInfos) =>
            {
                var criteria = WorkContext.CurrentProductSearchCriteria.Clone();
                criteria.PageNumber = pageNumber;
                criteria.PageSize = pSize;
                if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                {
                    criteria.SortBy = SortInfo.ToString(sortInfos);
                }
                return _catalogSearchService.SearchProducts(criteria).Products;
            }, page, pageSize);
        }
    }
}