using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.AutoRestClients.SearchApiModuleApi;
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
        private readonly ISearchApiModuleApiClient _searchApi;
        private readonly Func<WorkContext> _workContextFactory;
        private readonly ILocalCacheManager _cacheManager;
        private readonly ICoreModuleApiClient _coreApi;
        private readonly ICatalogModuleApiClient _catalogApi;
        private static Category _loadedCategory;
        private static Dictionary<string, Category> _seoCategoryDict = new Dictionary<string, Category>();
        private Language _language;
        private Currency _currency;
        private Store _store;

        public ESCategoryTreeService(ISearchApiModuleApiClient searchApi, Func<WorkContext> workContextFactory, ILocalCacheManager cacheManager, ICoreModuleApiClient coreApi, ICatalogModuleApiClient catalogApi)
        {
            _searchApi = searchApi;
            _cacheManager = cacheManager;
            _workContextFactory = workContextFactory;
            _coreApi = coreApi;
            _catalogApi = catalogApi;
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
                        .ContinueWith(t1 => LoadChildrenFromCategory(t1.Result, "Tags")));

            // Step 2. Load Cities
            await LoadChildrenFromCategory(new Category[] { _loadedCategory }, "Cities")
                .ContinueWith(t=> LoadChildrenFromCategory(t.Result, "Estatetypes")
                    .ContinueWith(t1 => LoadChildrenFromCategory(t1.Result, "Tags")));

            // Step 2. Load Estatetypes
            await LoadChildrenFromCategory(new Category[] { _loadedCategory }, "Estatetypes");

            // Step 3. Load Tags
            await LoadChildrenFromCategory(new Category[] { _loadedCategory }, "Tags");

            _lockObject.Release();
            return _loadedCategory;
        }

        private async Task<Category[]> LoadChildrenFromCategory(Category[] parents, string productType)
        {
            var result = await _searchApi.SearchApiModule.SearchProductsAsync(_storeId, 
                new AutoRestClients.SearchApiModuleApi.Models.ProductSearch
                {
                    ResponseGroup = "1619",
                    Outline = ProductTypeToOutline(productType),
                    Take = int.MaxValue,
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
                    return ConfigurationManager.AppSettings["RegionCategoryId"];
                case "Estatetypes":
                    return ConfigurationManager.AppSettings["EstateTypeCategoryId"];
                case "Tags":
                    return "d6cce7c7d9854f609ab3fd5109d79a57";
                case "Cities":
                    return ConfigurationManager.AppSettings["CityCategoryId"];
            }
            return string.Empty;
        }

        private Category ConvertProductToCategory(Category parent, string productType, AutoRestClients.SearchApiModuleApi.Models.Product dtoProduct)
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
                // TODO: Uncomment for load meta information in category
                //FillInfoPropertyForCategory(seoPath, category);
                _seoCategoryDict.Add(seoPath, category);
            }
            return category;
        }

        private void FillInfoPropertyForCategory(string seoPath, Category category)
        {
            if (seoPath == null)
            {
                throw new ArgumentNullException($"{nameof(seoPath)} is null");
            }
            if (category == null)
            {
                throw new ArgumentNullException($"{nameof(category)} is null");
            }

            var pathParts = seoPath.Split('/');

            foreach (var part in pathParts)
            {
                var seo = GetAllSeoRecords(part).FirstOrDefault(x => x.ObjectType == "CatalogProduct");
                var product = _cacheManager.Get($"Product{seo.ObjectId}", "SeoProducts", () => _catalogApi
                                                                                                .CatalogModuleProducts.GetProductById(seo.ObjectId, ItemResponseGroup.ItemMedium.ToString())
                                                                                                ?.ToProduct(_language,
                                                                                                            _currency,
                                                                                                            _store));
                category.RegionProduct = product;
                if (product.CategoryId == ConfigurationManager.AppSettings["RegionCategoryId"])
                {
                    category.RegionProduct = product;
                }
                else if (product.CategoryId == ConfigurationManager.AppSettings["TypeCategoryId"])
                {
                    category.TypeProduct = product;
                }
                else if (product.CategoryId == ConfigurationManager.AppSettings["CityCategoryId"])
                {
                    category.CityProduct = product;
                    // link to region
                    var regionAssociationId = product.Associations.OfType<ProductAssociation>().FirstOrDefault(x => x.Type == "Regions")?.ProductId;
                    if (!string.IsNullOrEmpty(regionAssociationId))
                    {
                        var regionProduct = _cacheManager.Get($"Product{regionAssociationId}", "SeoProducts", () => _catalogApi
                                                                                                .CatalogModuleProducts.GetProductById(regionAssociationId, ItemResponseGroup.ItemAssociations.ToString())
                                                                                                ?.ToProduct(_language,
                                                                                                            _currency,
                                                                                                            _store));
                        category.RegionProduct = regionProduct;
                    }
                }
            }
        }

        protected virtual IList<coreDto.SeoInfo> GetAllSeoRecords(string slug)
        {
            var result = new List<coreDto.SeoInfo>();

            if (!string.IsNullOrEmpty(slug))
            {
                var apiResult = _cacheManager.Get(string.Join(":", "Commerce.GetSeoInfoBySlug", slug), "ApiRegion", () => _coreApi.Commerce.GetSeoInfoBySlug(slug));
                result.AddRange(apiResult);
            }

            return result;
        }

        private ICategoryTreeConverter GetConverterByPath(string path)
        {
            return new DefaultCategoryTreeConverter();
        }

        public Category FindByPath(string path)
        {
            return _seoCategoryDict.ContainsKey(path) ? _seoCategoryDict[path] : null;
        }
    }
}