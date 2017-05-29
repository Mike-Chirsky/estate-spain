﻿using System.Globalization;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Omu.ValueInjecter;
using PagedList;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class CustomerConverter
    {
        public static Customer ToShopifyModel(this CustomerInfo customer, StorefrontModel.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var converter = ServiceLocator.Current.GetInstance<ShopifyModelConverter>();
            return converter.ToLiquidCustomer(customer, workContext, urlBuilder);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Customer ToLiquidCustomer(CustomerInfo customer, StorefrontModel.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new Customer();

            result.InjectFrom<NullableAndEnumValueInjecter>(customer);
            result.Name = customer.FullName;
            if(customer.DefaultAddress != null)
            {
                result.DefaultAddress = ToLiquidAddress(customer.DefaultAddress);
            }
            if(customer.DefaultBillingAddress != null)
            {
                result.DefaultBillingAddress = ToLiquidAddress(customer.DefaultBillingAddress);
            }
            if (customer.DefaultShippingAddress != null)
            {
                result.DefaultShippingAddress = ToLiquidAddress(customer.DefaultShippingAddress);
            }

            if (customer.Tags != null)
            {
                result.Tags = customer.Tags.ToList();
            }

            if (customer.Addresses != null)
            {
                var addresses = customer.Addresses.Select(a => ToLiquidAddress(a)).ToList();
                result.Addresses = new MutablePagedList<Address>(addresses);
            }

            if (customer.Orders != null)
            {
                result.Orders = new MutablePagedList<Order>((pageNumber, pageSize, sortInfos) =>
                {
                    customer.Orders.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<Order>(customer.Orders.Select(x =>ToLiquidOrder(x, workContext.CurrentLanguage, urlBuilder)), customer.Orders);
                }, customer.Orders.PageNumber, customer.Orders.PageSize);
            }

            if (customer.QuoteRequests != null)
            {
                result.QuoteRequests = new MutablePagedList<QuoteRequest>((pageNumber, pageSize, sortInfos) =>
                {
                    customer.QuoteRequests.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<QuoteRequest>(customer.QuoteRequests.Select(x => ToLiquidQuoteRequest(x)), customer.QuoteRequests);
                }, customer.QuoteRequests.PageNumber, customer.QuoteRequests.PageSize);
            }

            if (customer.DynamicProperties != null)
            {
                result.Metafields = new MetaFieldNamespacesCollection(new[] { new MetafieldsCollection("dynamic_properties", workContext.CurrentLanguage, customer.DynamicProperties) });
            }
            return result;
        }
    }
}
