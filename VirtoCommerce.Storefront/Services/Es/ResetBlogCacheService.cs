using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Services.Es
{
    public class ResetBlogCacheService : IResetBlogCacheService
    {
        private readonly ILocalCacheManager _cacheManager;
        public ResetBlogCacheService(ILocalCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void ClearBlogCache(string path, WorkContext workContext)
        {
            //clear output cache
            var outputCachePath = $"{workContext.CurrentStore.Id}/{workContext.CurrentLanguage.CultureName.ToLower()}/{path}";
            var cacheHandle = CacheManager.Web.CacheManagerOutputCacheProvider.Cache.CacheHandles.FirstOrDefault();
            var cache = (System.Collections.IEnumerable)cacheHandle.GetType().GetField("cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cacheHandle);

            foreach (var item in cache)
            {
                var key = item.GetType().GetProperty("Key", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(item).ToString();

                if (key.Contains(outputCachePath))
                {
                    var removeKey = string.Join(":", key.Split(':').Skip(1));
                    CacheManager.Web.CacheManagerOutputCacheProvider.Cache.Remove(removeKey);
                }
            }
            // reset product cache
            if (_cacheManager == null)
            {
                return;
            }
            _cacheManager.ClearRegion("ContentRegion");
        }
    }
}