using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Services.Es;

namespace VirtoCommerce.Storefront.Controllers.Api.Es
{
    public class ApiBlogCacheController : StorefrontControllerBase
    {
        private readonly IResetBlogCacheService _resetBlogCacheService;
        public ApiBlogCacheController(WorkContext context, IStorefrontUrlBuilder urlBuilder, IResetBlogCacheService resetBlogCacheService) : base(context, urlBuilder)
        {
            _resetBlogCacheService = resetBlogCacheService;
        }

        public ActionResult ClearBlogCache(string key, string url)
        {
            if (key != "15117b282328146ac6afebaa8acd80e7")
            {
                return new HttpStatusCodeResult(403);
            }
            _resetBlogCacheService.ClearBlogCache(url, WorkContext);
            return new HttpStatusCodeResult(200);
        }
    }
}