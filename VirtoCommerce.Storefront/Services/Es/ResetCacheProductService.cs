using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Services.Es
{
    public class ResetCacheProductService : IResetCacheProductService
    {
        private readonly Func<ICoreModuleApiClient> _coreApiFactory;
        private readonly ILocalCacheManager _cacheManager;
        public ResetCacheProductService(Func<ICoreModuleApiClient> coreApiFactory, ILocalCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _coreApiFactory = coreApiFactory;
        }
        public void ResetProductCache(string path, WorkContext workContex)
        {
            //clear output cache
            var outputCachePath = $"{workContex.CurrentStore.Id}/{workContex.CurrentLanguage.CultureName.ToLower()}/{path}";
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
            cacheHandle = _cacheManager.CacheHandles.FirstOrDefault();
            cache = (System.Collections.IEnumerable)cacheHandle.GetType().GetField("cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cacheHandle);
            var seo = _coreApiFactory().Commerce.GetSeoInfoBySlug(path)?.FirstOrDefault();
            if (seo != null)
            {
                foreach (var item in cache)
                {
                    var key = item.GetType().GetProperty("Key", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(item).ToString();
                    if (key.Contains(seo.ObjectId))
                    {
                        var removeKey = string.Join(":", key.Split(':').Skip(1));
                        _cacheManager.Remove(removeKey, "ApiRegion");
                    }
                }
            }
        }
    }
}