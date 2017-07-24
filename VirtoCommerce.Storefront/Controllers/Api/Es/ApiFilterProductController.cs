using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VirtoCommerce.Storefront.Converters.Catalog;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Es;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Services.Es;

namespace VirtoCommerce.Storefront.Controllers.Api.Es
{
    public class ApiFilterProductController : StorefrontControllerBase
    {
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly ICategoryTreeService _categoryTreeService;

        public ApiFilterProductController(WorkContext context, IStorefrontUrlBuilder urlBuilder, ICatalogSearchService catalogSearchService, ICategoryTreeService categoryTreeService) : base(context, urlBuilder)
        {
            _catalogSearchService = catalogSearchService;
            _categoryTreeService = categoryTreeService;
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

        // storefrontapi/product/filter/checkurl

        /// <summary>
        /// Check exits url if not exist return valid url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckExsitUrl(CheckUrlRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Url))
            {
                return Json(false);
            }
            var uri = new Uri(request.Url);
            var url = uri.AbsolutePath.Trim('/');
            
            if (_categoryTreeService.FindByPath(url) == null)
            {
                var paths = url.Split('/');
                Category existCategory = null;
                do
                {
                    var tmp = "";
                    for (int i = 0; i < paths.Length - 1; i++)
                    {
                        tmp += "/" + paths[i];
                    }
                    tmp = tmp.Trim('/');
                    if (string.IsNullOrEmpty(tmp) || (existCategory = _categoryTreeService.FindByPath(tmp)) != null)
                    {
                        break;
                    }

                }
                while (true);
                if (existCategory != null)
                {
                    return Json(CreateUrl(existCategory, request.Terms));
                }
                else
                {
                    return Json(CreateSearchUrl(request.Terms));
                }
            }
            else
            {
                return Json(true);
            }
        }

        private string CreateSearchUrl(ProductFilterCriteria criteria)
        {
            return $"search?{CreateParams(new string[0], criteria)}";
        }

        private string CreateUrl(Category category, ProductFilterCriteria criteria)
        {
            switch (category.Type)
            {
                case "region":
                    return $"{category.SeoPath}?{CreateParams(new[] { "region" }, criteria)}";
                case "city":
                    return $"{category.SeoPath}?{CreateParams(new[] { "region", "city" }, criteria)}";
                case "type":
                    return $"{category.SeoPath}?{CreateParams(new[] { "estatetype" }, criteria)}";
                case "type_add":
                    return $"{category.SeoPath}?{CreateParams(new[] { "region", "city", "estatetype" }, criteria)}";
                case "tag":
                    return $"{category.SeoPath}?{CreateParams(new[] { "tag" }, criteria)}";
                case "tag_add":
                    if (category.Path == "/Cities/Estatetypes/Tags")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "tag", "city", "estatetype" }, criteria)}";
                    }
                    else if (category.Path == "/Cities/Tags")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "tag", "city" }, criteria)}";
                    }
                    else if (category.Path == "/Estatetypes/Tags")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "tag", "estatetype" }, criteria)}";
                    }
                    else if (category.Path == "/Regions/Estatetypes/Tags")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "tag", "estatetype", "region" }, criteria)}";
                    }
                    else if (category.Path == "/Regions/Tags")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "tag", "region" }, criteria)}";
                    }
                    break;
                case "condition":
                    return $"{category.SeoPath}?{CreateParams(new[] { "cond" }, criteria)}";
                case "condition_add":
                    if (category.Path == "/Cities/Conditions")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "cond", "city" }, criteria)}";
                    }
                    else if (category.Path == "/Cities/Estatetypes/Conditions")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "cond", "city", "estatetype" }, criteria)}";
                    }
                    else if (category.Path == "/Regions/Conditions")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "cond", "region" }, criteria)}";
                    }
                    else if (category.Path == "/Regions/Estatetypes/Conditions")
                    {
                        return $"{category.SeoPath}?{CreateParams(new[] { "cond", "region", "estatetype" }, criteria)}";
                    }
                    break;
                case "other_type":
                    return $"{category.SeoPath}?{CreateParams(new[] { "type" }, criteria)}";
            }
            return category.SeoPath;
        }

        private string CreateParams(string[] exclude, ProductFilterCriteria criteria)
        {
            var result = string.Empty;
            foreach (var prop in criteria.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propName = prop.Name.ToLower();
                if (!string.IsNullOrEmpty(exclude.FirstOrDefault(x => x == propName)))
                {
                    continue;
                }
                var vl = prop.GetValue(criteria);
                if (vl != null)
                {
                    result += $"{propName}={prop.GetValue(criteria)}&";
                }
            }
            return result.Trim('&');
        }
    }
}