using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Es.Search;
using VirtoCommerce.Storefront.Model.Catalog;
using System.Configuration;

namespace VirtoCommerce.Storefront.Converters.Es
{
    public static class ProductToLocationItem
    {
        public static LocationSearchItem ToLocationItem(this Product product)
        {
            var result = new LocationSearchItem();
            if (product.CategoryId == ConfigurationManager.AppSettings["RegionCategoryId"])
            {
                result.RegionName = product.Name;
                result.RegionSeo = product.SeoInfo?.Slug;
            }
            if (product.CategoryId == ConfigurationManager.AppSettings["CityCategoryId"])
            {
                result.CityName = product.Name;
                result.CitySeo = product.SeoInfo?.Slug;
                var regionProduct = product.Associations.OfType<ProductAssociation>().FirstOrDefault(x => x.Type == "Regions")?.Product;
                if (regionProduct != null)
                {
                    result.RegionName = regionProduct.Name;
                    result.RegionSeo = regionProduct.SeoInfo?.Slug;
                }
            }
            return result;
        }

    }
}