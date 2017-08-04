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
        public ApiCategoryTreeController(WorkContext context, IStorefrontUrlBuilder urlBuilder, ICategoryTreeService categoryTreeService) : base(context, urlBuilder)
        {
            _categoryTreeService = categoryTreeService;
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
        public async Task<ActionResult> RegenerateElemet(string key, string path)
        {
            path = path.Replace("_", "/");
            if (key != "15117b282328146ac6afebaa8acd80e7")
            {
                return new HttpStatusCodeResult(403);
            }
            await _categoryTreeService.RebuildElement(path);
            return new HttpStatusCodeResult(200);
        }
    }
}