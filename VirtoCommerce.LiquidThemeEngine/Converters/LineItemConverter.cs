﻿using Microsoft.Practices.ServiceLocation;
using Omu.ValueInjecter;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class LineItemConverter
    {
        public static LineItem ToShopifyModel(this StorefrontModel.Cart.LineItem lineItem, StorefrontModel.Language language, IStorefrontUrlBuilder urlBuilder)
        {
            var converter = ServiceLocator.Current.GetInstance<ShopifyModelConverter>();
            return converter.ToLiquidLineItem(lineItem, language, urlBuilder);
        }

        public static LineItem ToShopifyModel(this StorefrontModel.Order.LineItem lineItem, StorefrontModel.Language language, IStorefrontUrlBuilder urlBuilder)
        {
            var converter = ServiceLocator.Current.GetInstance<ShopifyModelConverter>();
            return converter.ToLiquidLineItem(lineItem, language, urlBuilder);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual LineItem ToLiquidLineItem(StorefrontModel.Cart.LineItem lineItem, StorefrontModel.Language language, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new LineItem();

            result.InjectFrom<StorefrontModel.Common.NullableAndEnumValueInjecter>(lineItem);

            //shopifyModel.Product = lineItem.Product.ToShopifyModel();
            result.Fulfillment = null; // TODO
            result.Grams = lineItem.Weight ?? 0m;
            result.Image = new Image
            {
                Alt = lineItem.Name,
                Name = lineItem.Name,
                ProductId = lineItem.ProductId,
                Src = lineItem.ImageUrl
            };
            result.LinePrice = lineItem.ExtendedPrice.Amount * 100;
            result.LinePriceWithTax = lineItem.ExtendedPriceWithTax.Amount * 100;
            result.Price = lineItem.PlacedPrice.Amount * 100;
            result.PriceWithTax = lineItem.PlacedPriceWithTax.Amount * 100;
            result.Title = lineItem.Name;
            result.VariantId = lineItem.ProductId;

            result.Properties = new MetafieldsCollection("properties", language, lineItem.DynamicProperties);

            return result;
        }

        public virtual LineItem ToLiquidLineItem(StorefrontModel.Order.LineItem lineItem, StorefrontModel.Language language, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new LineItem();

            result.InjectFrom<StorefrontModel.Common.NullableAndEnumValueInjecter>(lineItem);

            result.Fulfillment = null; // TODO
            result.Grams = lineItem.Weight ?? 0m;
            result.Image = new Image
            {
                Alt = lineItem.Name,
                Name = lineItem.Name,
                ProductId = lineItem.ProductId,
                Src = lineItem.ImageUrl
            };
            result.LinePrice = lineItem.ExtendedPrice.Amount * 100;
            result.LinePriceWithTax = lineItem.ExtendedPriceWithTax.Amount * 100;
            result.Price = lineItem.PlacedPrice.Amount * 100;
            result.PriceWithTax = lineItem.PlacedPriceWithTax.Amount * 100;
            result.Title = lineItem.Name;
            result.Type = lineItem.ObjectType; 
            result.Url = urlBuilder.ToAppAbsolute("/product/" + lineItem.ProductId);
            result.Product = new Product
            {
                Id = result.ProductId,
                Url = result.Url
            };       

            return result;
        }
    }
}