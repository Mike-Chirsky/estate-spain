using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Es;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Services.Es.Converters;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model;
using System.Threading;
using System.Configuration;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Services.Es
{
    public class ESCategoryTreeService: ICategoryTreeService
    {
        private const string _storeId = "MasterStore";
        private readonly SemaphoreSlim _lockObject = new SemaphoreSlim(1);
        private readonly ICatalogModuleApiClient _catalogModuleApi;
        private readonly Func<WorkContext> _workContextFactory;
        private static Category _loadedCategory;
        private static Dictionary<string, Category> _seoCategoryDict = new Dictionary<string, Category>();
        private Language _language;
        private Currency _currency;
        private Store _store;

        public ESCategoryTreeService(ICatalogModuleApiClient catalogModuleApi, Func<WorkContext> workContextFactory)
        {
            _catalogModuleApi = catalogModuleApi;
            _workContextFactory = workContextFactory;
        }

        public async Task<Category> GetTree()
        {
            await _lockObject.WaitAsync();
            if (_loadedCategory != null)
            {
                _lockObject.Release();
                return _loadedCategory;
            }
            _loadedCategory = new Category();
            // init data for product converter
            var wc = _workContextFactory();
            _language = wc.CurrentLanguage;
            _currency = wc.CurrentCurrency;
            _store = wc.CurrentStore;

            // Step 1. Load Regions
            // Step 1.1 Load Regions/Estatetypes
            // Step 1.2 Load Regions/Estatetypes/Tags
            await LoadChildrenFromCategory(new Category[] { _loadedCategory }, "Regions")
                    .ContinueWith(t => LoadChildrenFromCategory(t.Result, "Estatetypes")
                        .ContinueWith(t2 => LoadChildrenFromCategory(t2.Result, "Tags")));

            // Step 2. Load Cities
            await LoadCities(_loadedCategory.Categories.Where(x => x.ProductType == "Regions").ToArray())
                    .ContinueWith(t => LoadChildrenFromCategory(t.Result, "Estatetypes")
                        .ContinueWith(t2 => LoadChildrenFromCategory(t2.Result, "Tags")));

            // Step 2. Load Estatetypes
            await LoadChildrenFromCategory(new Category[] { _loadedCategory }, "Estatetypes");

            // Step 3. Load Tags
            await LoadChildrenFromCategory(new Category[] { _loadedCategory }, "Tags");

            _lockObject.Release();
            return _loadedCategory;
        }

        private async Task<Category[]> LoadChildrenFromCategory(Category[] parents, string productType)
        {
            var result = await _catalogModuleApi.CatalogModuleSearch.SearchProductsAsync(
                new AutoRestClients.CatalogModuleApi.Models.ProductSearchCriteria
                {
                    CatalogId = ConfigurationManager.AppSettings["MasterCatalogId"],
                    ResponseGroup = "1619",
                    Outline = ProductTypeToOutline(productType),
                    Take = 10000,
                    Skip = 0
                });

            foreach (var parent in parents)
            {
                var categories = new List<Category>();

                if (result.Items != null)
                {

                    var children = result.Items.Select(p => ConvertProductToCategory(parent, productType, p));
                    if (parent.Categories != null)
                        categories.AddRange(parent.Categories);
                    categories.AddRange(children);
                }

                parent.Categories = new MutablePagedList<Category>(categories);
            }

            return parents.SelectMany( p => p.Categories).ToArray();
        }

        private async Task<Category[]> LoadCities(Category[] parents)
        {
            var resultCities = await _searchApi.SearchApiModule.SearchProductsAsync(_storeId,
                new AutoRestClients.SearchApiModuleApi.Models.ProductSearch
                {
                    ResponseGroup = (ItemResponseGroup.ItemAssociations | ItemResponseGroup.Seo | ItemResponseGroup.ItemEditorialReviews | ItemResponseGroup.ItemInfo).ToString(),
                    Outline = ProductTypeToOutline("Cities"),
                    Take = int.MaxValue,
                    Skip = 0
                });
            foreach (var parent in parents)
            {
                var categories = new List<Category>();

                var children = resultCities.Products.Where(x => x.Associations.Any(a => a.AssociatedObjectId == parent.Id)).Select(p => ConvertProductToCategory(parent, "Cities", p));
                categories.AddRange(children);
                parent.Categories = new MutablePagedList<Category>(categories);
            }
            return parents.SelectMany(x => x.Categories).ToArray();
        }

        private string ProductTypeToOutline(string productType)
        {
            switch(productType)
            {
                case "Regions":
                    return ConfigurationManager.AppSettings["RegionCategoryId"];
                case "Estatetypes":
                    return ConfigurationManager.AppSettings["EstateTypeCategoryId"];
                case "Tags":
                    return ConfigurationManager.AppSettings["TagCategoryId"];
                case "Cities":
                    return ConfigurationManager.AppSettings["CityCategoryId"];
            }
            return string.Empty;
        }

        private Category ConvertProductToCategory(Category parent, string productType, AutoRestClients.CatalogModuleApi.Models.Product dtoProduct)
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
            var seoPath = category.SeoPath.Trim('/');
            if (!_seoCategoryDict.ContainsKey(seoPath))
            {
                _seoCategoryDict.Add(seoPath, category);
            }
            return category;
        }
        
        private ICategoryTreeConverter GetConverterByPath(string path)
        {
            if (path.Contains("Tag"))
            {
                return new TagCategoryTreeConverter();
            }
            else if (path.Contains("Estatetype"))
            {
                return new TypeCategoryTreeConverter();
            }
            else if (path.Contains("Cities"))
            {
                return new CityCategoryTreeConverter();
            }
            else if (path.Contains("Regions"))
            {
                return new RegionCategoryTreeConverter();
            }
            return new DefaultCategoryTreeConverter();
        }

        public Category FindByPath(string path)
        {
            return _seoCategoryDict.ContainsKey(path) ? _seoCategoryDict[path] : null;
        }
    }
}