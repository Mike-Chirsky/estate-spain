using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.AutoRestClients.SearchApiModuleApi;
using VirtoCommerce.Storefront.Model.Catalog.Es;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Services.Es.Converters;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Services.Es
{
    public class ESCategoryTreeService
    {
        private const string _storeId = "MasterStore";
        private readonly ISearchApiModuleApiClient _searchApi;
        private readonly Language _language;
        private readonly Currency _currency;
        private readonly Store _store;

        public ESCategoryTreeService(ISearchApiModuleApiClient searchApi, Language language, Currency currency, Store store)
        {
            _searchApi = searchApi;
            _language = language;
            _currency = currency;
            _store = store;
        }

        public async Task<Category> GetTree()
        {
            var root = new Category
            {
            };

            // Step 1. Load Regions
            // Step 1.1 Load Regions/Estatetypes
            // Step 1.2 Load Regions/Estatetypes/Tags
            await LoadChildrenFromCategory(new Category[] { root }, "Regions")
                    .ContinueWith(t => LoadChildrenFromCategory(t.Result, "Estatetypes")
                        .ContinueWith(t1 => LoadChildrenFromCategory(t1.Result, "Tags")));

            // Step 2. Load Cities
            await LoadChildrenFromCategory(new Category[] { root }, "Cities");

            // Step 2. Load Estatetypes
            await LoadChildrenFromCategory(new Category[] { root }, "Estatetypes");

            // Step 3. Load Tags
            await LoadChildrenFromCategory(new Category[] { root }, "Tags");

            return root;
        }

        private async Task<Category[]> LoadChildrenFromCategory(Category[] parents, string productType)
        {
            var result = await _searchApi.SearchApiModule.SearchProductsAsync(_storeId, 
                new AutoRestClients.SearchApiModuleApi.Models.ProductSearch
                {
                    ResponseGroup = "1619",
                    Outline = ProductTypeToOutline(productType),
                    Take = 5,
                    Skip = 0
                });

            foreach (var parent in parents)
            {
                var categories = new List<Category>();

                var children = result.Products.Select(p => ConvertProductToCategory(parent, productType, p));
                if(parent.Categories!=null)
                    categories.AddRange(parent.Categories);
                categories.AddRange(children);

                parent.Categories = new MutablePagedList<Category>(categories);
            }

            return parents.SelectMany( p => p.Categories).ToArray();
        }

        private string ProductTypeToOutline(string productType)
        {
            switch(productType)
            {
                case "Regions":
                    return "654dbd245eb2484fa5ccd8ffd69387da";
                case "Estatetypes":
                    return "2036643b08794d149e1722adbe0230e8";
                case "Tags":
                    return "d6cce7c7d9854f609ab3fd5109d79a57";
                case "Cities":
                    return "06d09840154341a4b4564aa9b94b06b3";
            }
            return string.Empty;
        }

        private Category ConvertProductToCategory(Category parent, string productType, VirtoCommerce.Storefront.AutoRestClients.SearchApiModuleApi.Models.Product dtoProduct)
        {
            var path = string.Join("/", parent.Path, productType);

            // TODO: Find Converter By Seo Path
            var converter = GetConverterByPath(path);

            var product = dtoProduct.ToProduct(_language, _currency, _store);

            var category = converter.ToCategory(new ConverterContext
                {
                    Parent = parent,
                    Path = path,
                    ProductType = productType
                }, product);

            return category;
        }

        private ICategoryTreeConverter GetConverterByPath(string path)
        {
            return new DefaultCategoryTreeConverter();
        }
    }
}