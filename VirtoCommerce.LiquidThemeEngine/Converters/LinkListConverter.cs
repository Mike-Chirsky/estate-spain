﻿using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Omu.ValueInjecter;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using StorefrontModel = VirtoCommerce.Storefront.Model;


namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class LinkListConverter
    {
        public static Linklist ToShopifyModel(this StorefrontModel.MenuLinkList linkList, Storefront.Model.WorkContext workContext,  IStorefrontUrlBuilder urlBuilder)
        {
            var converter = ServiceLocator.Current.GetInstance<ShopifyModelConverter>();
            return converter.ToLiquidLinklist(linkList, workContext, urlBuilder);
        }

        public static Link ToShopfiyModel(this StorefrontModel.MenuLink link, Storefront.Model.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var converter = ServiceLocator.Current.GetInstance<ShopifyModelConverter>();
            return converter.ToLiquidLink(link, workContext, urlBuilder);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Linklist ToLiquidLinklist(StorefrontModel.MenuLinkList linkList, Storefront.Model.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new Linklist();
            result.InjectFrom<StorefrontModel.Common.NullableAndEnumValueInjecter>(linkList);

            result.Handle = linkList.Name;
            result.Links = linkList.MenuLinks.Select(ml => ToLiquidLink(ml, workContext, urlBuilder)).ToList();
            result.Title = linkList.Name;

            return result;
        }

        public virtual Link ToLiquidLink(StorefrontModel.MenuLink link, Storefront.Model.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new Link();
            result.InjectFrom<StorefrontModel.Common.NullableAndEnumValueInjecter>(link);

            result.Object = "";
            result.Title = link.Title;
            result.Type = "";
            result.Url = urlBuilder.ToAppAbsolute(link.Url);

            var productLink = link as StorefrontModel.ProductMenuLink;
            var categoryLink = link as StorefrontModel.CategoryMenuLink;
            if (productLink != null)
            {
                result.Type = "product";
                if (productLink.Product != null)
                {
                    result.Object = productLink.Product.ToShopifyModel();
                }
            }
            if (categoryLink != null)
            {
                result.Type = "collection";
                if (categoryLink.Category != null)
                {
                    result.Object = categoryLink.Category.ToShopifyModel(workContext);
                }
            }
            return result;
        }
    }
}