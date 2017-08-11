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
    public class ApiProductCacheController : StorefrontControllerBase
    {
        private readonly IResetCacheProductService _resetProductCacheService;
        public ApiProductCacheController(WorkContext context, IStorefrontUrlBuilder urlBuilder, IResetCacheProductService resetCacheService) : base(context, urlBuilder)
        {
            _resetProductCacheService = resetCacheService;
        }

        public ActionResult ResetProductCache(string key, string url)
        {
            if (key != "15117b282328146ac6afebaa8acd80e7")
            {
                return new HttpStatusCodeResult(403);
            }
            _resetProductCacheService.ResetProductCache(url, WorkContext);
            return new HttpStatusCodeResult(200);
        }
    }
}