using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.LiquidThemeEngine.Converters.Extentsions
{
    public partial class EsShopifyModelConverter
    {
        public override Collection ToLiquidCollection(Category category, WorkContext workContext)
        {
            var collection = base.ToLiquidCollection(category, workContext);
            collection.CurrentTagCollection = new TagCollection(
                        workContext.CurrentProductSearchCriteria.Terms.Select(t => ToLiquidTag(t)).ToList());
            if (category.Properties != null)
            {
                collection.Properties = category.Properties.Select(x => x.ToShopifyModel()).ToList();
            }
            if (category.Descriptions != null)
            {
                collection.Descriptions = new Descriptions(category.Descriptions.Select(x => new Description
                {
                    Content = x.Value,
                    Type = x.ReviewType
                }));
            }
            collection.Type = category.Type;
            collection.ProductType = category.ProductType;
            collection.FullName = category.FullName;
            collection.CityUrl = category.CityUrl;
            collection.RegionUrl = category.RegionUrl;
            collection.CityName = category.CityName;
            collection.ParentCollectionImages = category.Parent?.Images.Select(x => x.ToShopifyModel()).ToArray();
            collection.RegionName = category.RegionName;
            collection.Breadcrumb = new List<KeyValue<string, string>>();

            var parent = category;
            do
            {
                collection.Breadcrumb.Add(new KeyValue<string, string>(parent.Name, parent.SeoPath));
                parent = parent.Parent;
            } while (parent != null && !string.IsNullOrEmpty(parent.Id));
            collection.Breadcrumb = collection.Breadcrumb.Reverse().ToList();
            return collection;
        }
    }
}
