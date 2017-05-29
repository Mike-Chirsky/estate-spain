﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiBlogController : StorefrontControllerBase
    {
        public ApiBlogController(WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
            : base(workContext, urlBuilder)
        {
        }

        // POST: storefrontapi/blog/{blogName}/search
        [HttpPost]
        public ActionResult Search(string blogName, BlogSearchCriteria criteria)
        {
            var articles = new List<BlogArticle>();

            var blog = WorkContext.Blogs.FirstOrDefault(b => b.Name.Equals(blogName, StringComparison.OrdinalIgnoreCase));
            if (blog != null)
            {
                var query = blog.Articles.AsQueryable().Where(x => x.IsPublished);
                if (!string.IsNullOrEmpty(criteria.Category))
                {
                    query = query.Where(a => !string.IsNullOrEmpty(a.Category) && a.Category.Handelize().EqualsInvariant(criteria.Category));
                }
                if (!string.IsNullOrEmpty(criteria.Tag))
                {
                    query = query.Where(a => a.Tags != null && a.Tags.Select(t => t.Handelize()).Contains(criteria.Tag, StringComparer.OrdinalIgnoreCase));
                }
                if (criteria.ExcludedArticleHandles != null && criteria.ExcludedArticleHandles.Any())
                {
                    query = query.Where(a => !criteria.ExcludedArticleHandles.Contains(a.Url));
                }

                articles = query.OrderByDescending(a => a.CreatedDate).Skip((criteria.PageNumber - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();
            }

            return Json(articles, JsonRequestBehavior.AllowGet);
        }
    }
}