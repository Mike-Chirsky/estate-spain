using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Owin;
using VirtoCommerce.Storefront.Services.Es;

namespace VirtoCommerce.Storefront.Controllers.Api.Es
{
    public class ApiCategoryTreeController : StorefrontControllerBase
    {
        private readonly ICategoryTreeService _categoryTreeService;
        private readonly ILocalCacheManager _localCacheManager;
        public ApiCategoryTreeController(WorkContext context, IStorefrontUrlBuilder urlBuilder, ICategoryTreeService categoryTreeService, ILocalCacheManager localCacheManager) : base(context, urlBuilder)
        {
            _categoryTreeService = categoryTreeService;
            _localCacheManager = localCacheManager;
        }

        //  GET: /storefrontapi/categorytree/rebuild/{key}
        public async Task<ActionResult> RegenerateTree(string key)
        {
            if (key != "15117b282328146ac6afebaa8acd80e7")
            {
                return new HttpStatusCodeResult(403);
            }
            await _categoryTreeService.BuildTree();
            WorkContextOwinMiddleware.LastExceptionBuildTree = null;
            WorkContextOwinMiddleware.LastTimeFialBuildTree = null;
            return new HttpStatusCodeResult(200);
        }
        //  GET: /storefrontapi/categorytree/rebuild/element/{key}/{path}
        public async Task<ActionResult> RegenerateElemet(string key, string url)
        {
            if (key != "15117b282328146ac6afebaa8acd80e7")
            {
                return new HttpStatusCodeResult(403);
            }
            await _categoryTreeService.RebuildElement(url);
            return new HttpStatusCodeResult(200);
        }

        private void WriteCacheKeys()
        {
            var cacheHandle = _localCacheManager.CacheHandles.FirstOrDefault();
            var cache = (System.Collections.IEnumerable)cacheHandle.GetType().GetField("cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cacheHandle);
            var listKeys = new List<string>();
            foreach (var item in cache)
            {
                listKeys.Add(item.GetType().GetProperty("Key", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(item).ToString());
            }
            using (var file = new System.IO.StreamWriter(System.IO.File.Create(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache.txt"))))
            {
                foreach (var item in listKeys)
                {
                    file.WriteLine(item);
                }
            }
        }
    }
}