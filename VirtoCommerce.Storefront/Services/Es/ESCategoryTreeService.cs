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
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using System.Diagnostics;
using System.Collections.Concurrent;
using VirtoCommerce.Storefront.Owin;
using VirtoCommerce.Storefront.Routing;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;

namespace VirtoCommerce.Storefront.Services.Es
{
    public class ESCategoryTreeService : ICategoryTreeService
    {
        private readonly SemaphoreSlim _lockObject = new SemaphoreSlim(1);
        private readonly ICatalogModuleApiClient _catalogModuleApi;
        private readonly Func<WorkContext> _workContextFactory;
        private readonly Func<ICoreModuleApiClient> _coreApiFactory;
        private static Dictionary<string, Category> _seoCategoryDict;
        private static ConcurrentBag<Exception> _buildTreeExceptions = new ConcurrentBag<Exception>();
        private Language _language;
        private Currency _currency;
        private Store _store;
        private readonly ILocalCacheManager _cacheManager;

        public const string RegionKey = "Regions";
        public const string EstateTypeKey = "Estatetypes";
        public const string TagsKey = "Tags";
        public const string ConditionKey = "Conditions";
        public const string OtherTypeKey = "OtherType";
        public const string CitiesKey = "Cities";
        public const string SinglePageKey = "SinglePage";

        public ESCategoryTreeService(ICatalogModuleApiClient catalogModuleApi, Func<WorkContext> workContextFactory, ILocalCacheManager cacheManager, Func<ICoreModuleApiClient> coreApiFactory)
        {
            _catalogModuleApi = catalogModuleApi;
            _workContextFactory = workContextFactory;
            _cacheManager = cacheManager;
            _coreApiFactory = coreApiFactory;
        }

        public async Task<Dictionary<string, Category>> GetTree()
        {
            if (_seoCategoryDict != null)
            {
                return _seoCategoryDict;
            }
            try
            {
                await _lockObject.WaitAsync();
                if (_seoCategoryDict != null)
                {
                    return _seoCategoryDict;
                }
                return await GenerateTree();
            }
            finally
            {
                _lockObject.Release();
            }
        }

        public async Task<Dictionary<string, Category>> RebuildTree()
        {
            try
            {
                await _lockObject.WaitAsync();
                var result = await GenerateTree();
                ClearCache(_seoCategoryDict.Keys.ToList());
                WorkContextOwinMiddleware.FilterSeo = null;
                return result;
            }
            finally
            {
                _lockObject.Release();
            }
        }

        private async Task<Dictionary<string, Category>> GenerateTree()
        {
            var tempSeoDict = new Dictionary<string, Category>();
            // init data for product converter
            var wc = _workContextFactory();
            _language = wc.CurrentLanguage;
            _currency = wc.CurrentCurrency;
            _store = wc.CurrentStore;
            // Load region + estate type
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, EstateTypeKey, tempSeoDict));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), EstateTypeKey, tempSeoDict);
            // Load region + tags
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, TagsKey, tempSeoDict));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), TagsKey, tempSeoDict);
            // Load region + condition
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, ConditionKey, tempSeoDict));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), ConditionKey, tempSeoDict);
            // Load region + estate type + tags
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, EstateTypeKey, tempSeoDict).ContinueWith(t1 => LoadChildrenFromCategory(t1.Result, TagsKey, tempSeoDict)));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), EstateTypeKey, tempSeoDict), TagsKey, tempSeoDict);
            // Load region + estate type + condition
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, EstateTypeKey, tempSeoDict).ContinueWith(t1 => LoadChildrenFromCategory(t1.Result, ConditionKey, tempSeoDict)));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), EstateTypeKey, tempSeoDict), ConditionKey, tempSeoDict);
            // Load cities + tags
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t1 => HandleFailTask(t1, LoadCities(t1.Result, tempSeoDict)).ContinueWith(t => HandleFailTask(t, LoadChildrenFromCategory(t.Result, TagsKey, tempSeoDict)).ContinueWith(t2 => HandleFailTask(t2, null))));
            await LoadChildrenFromCategory(await LoadCities(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), tempSeoDict), TagsKey, tempSeoDict);
            // load cities + condition
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t1 => LoadCities(t1.Result, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, ConditionKey, tempSeoDict)));
            await LoadChildrenFromCategory(await LoadCities(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), tempSeoDict), ConditionKey, tempSeoDict);
            // load cities + estate type
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t1 => LoadCities(t1.Result, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, EstateTypeKey, tempSeoDict)));
            await LoadChildrenFromCategory(await LoadCities(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), tempSeoDict), EstateTypeKey, tempSeoDict);
            
            // Load cities + estate type + conditions
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t1 => LoadCities(t1.Result, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, EstateTypeKey, tempSeoDict).ContinueWith(t2 => LoadChildrenFromCategory(t2.Result, ConditionKey, tempSeoDict))));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(await LoadCities(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), tempSeoDict), EstateTypeKey, tempSeoDict), ConditionKey, tempSeoDict);
            // Load cities + estate type + tags
            //await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict).ContinueWith(t1 => LoadCities(t1.Result, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, EstateTypeKey, tempSeoDict).ContinueWith(t2 => LoadChildrenFromCategory(t2.Result, TagsKey, tempSeoDict))));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(await LoadCities(await LoadChildrenFromCategory(new Category[] { new Category() }, RegionKey, tempSeoDict), tempSeoDict), EstateTypeKey, tempSeoDict), TagsKey, tempSeoDict);

            // Step 2. Load Estatetypes
            await LoadChildrenFromCategory(new Category[] { new Category() }, EstateTypeKey, tempSeoDict);


            // estatetype + tag
            //await LoadChildrenFromCategory(new Category[] { new Category() }, EstateTypeKey, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, TagsKey, tempSeoDict));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(new Category[] { new Category() }, EstateTypeKey, tempSeoDict), TagsKey, tempSeoDict);
            // estatetype + Condition
            //await LoadChildrenFromCategory(new Category[] { new Category() }, EstateTypeKey, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, ConditionKey, tempSeoDict));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(new Category[] { new Category() }, EstateTypeKey, tempSeoDict), ConditionKey, tempSeoDict);

            // Step 3. Load Tags
            await LoadChildrenFromCategory(new Category[] { new Category() }, TagsKey, tempSeoDict);

            // Step 4. Load Conditions
            await LoadChildrenFromCategory(new Category[] { new Category() }, ConditionKey, tempSeoDict);

            // Condition + tag
            //await LoadChildrenFromCategory(new Category[] { new Category() }, ConditionKey, tempSeoDict).ContinueWith(t => LoadChildrenFromCategory(t.Result, TagsKey, tempSeoDict));
            await LoadChildrenFromCategory(await LoadChildrenFromCategory(new Category[] { new Category() }, ConditionKey, tempSeoDict), TagsKey, tempSeoDict);
            // Step 5. Load Other type
            await LoadChildrenFromCategory(new Category[] { new Category() }, OtherTypeKey, tempSeoDict);

            // Step 6 Load Single pages
            await LoadChildrenFromCategory(new Category[] { new Category() }, SinglePageKey, tempSeoDict);
            _seoCategoryDict = tempSeoDict;
            return _seoCategoryDict;
        }

        private async Task<Category[]> LoadChildrenFromCategory(Category[] parents, string productType, Dictionary<string, Category> seoDict)
        {
            var result = await _catalogModuleApi.CatalogModuleSearch.SearchProductsAsync(
                new AutoRestClients.CatalogModuleApi.Models.ProductSearchCriteria
                {
                    CatalogId = ConfigurationManager.AppSettings["MasterCatalogId"],
                    ResponseGroup = "1619",
                    Outline = ProductTypeToOutline(productType),
                    Take = 10000,
                    Skip = 0,
                    WithHidden = false
                });
            var listExceptions = new List<Product>();
            var firstParent = parents.FirstOrDefault();
            if (firstParent != null)
            {
                listExceptions = await GetExceptionProducts(string.Join("/", firstParent.Path, productType));
            }
            foreach (var parent in parents)
            {
                var categories = new List<Category>();

                if (result.Items != null)
                {

                    var children = result.Items.Select(p => ConvertProductToCategory(parent, productType, p, listExceptions, seoDict)).Where(x => x != null);
                    if (parent.Categories != null)
                        categories.AddRange(parent.Categories);
                    categories.AddRange(children);
                }

                parent.Categories = new MutablePagedList<Category>(categories);
            }
            return parents.SelectMany(p => p.Categories).ToArray();
        }

        private async Task<Category[]> LoadCities(Category[] parents, Dictionary<string, Category> seoDict)
        {
           var resultCities = await _catalogModuleApi.CatalogModuleSearch.SearchProductsAsync(
               new AutoRestClients.CatalogModuleApi.Models.ProductSearchCriteria
               {
                   CatalogId = ConfigurationManager.AppSettings["MasterCatalogId"],
                   ResponseGroup = $"{CreateProductResponseGroup()} | ReferencedAssociations",
                   Outline = ProductTypeToOutline(CitiesKey),
                   Take = 10000,
                   Skip = 0,
                   WithHidden = false
               });
            foreach (var parent in parents)
            {
                var categories = new List<Category>();

                if (resultCities.Items != null)
                {
                    var children = resultCities.Items.Where(x => x != null && x.Associations != null && x.Associations.Any(a => a.AssociatedObjectId == parent.Id))
                                    .Select(p => ConvertProductToCategory(parent, "Cities", p, new List<Product>(), seoDict))
                                    .Where(x => x != null).ToList();
                    categories.AddRange(children);
                }
                parent.Categories = new MutablePagedList<Category>(categories);
            }
            var cities = parents.SelectMany(x => x.Categories);
            // fill child cities
            foreach (var city in cities)
            {
                var itemCity = resultCities.Items.FirstOrDefault(x => x.Id == city.Id);
                var referIds = itemCity.ReferencedAssociations.Where(x => x.Type == "Cities").Select(x => x.AssociatedObjectId);
                city.ChildCategory = cities.Where(x => referIds.Contains(x.Id)).ToList();
                foreach (var child in city.ChildCategory)
                {
                    child.Parent = city;
                }
                if (city.ChildCategory.Count != referIds.Count())
                {
                    foreach (var cityRefId in referIds)
                    {
                        if (!city.ChildCategory.Any(x => x.Id == cityRefId))
                        {
                            city.ChildCategory.Add(new Category
                            {
                                ProductType = CitiesKey,
                                Name = itemCity.ReferencedAssociations.FirstOrDefault(x => x.AssociatedObjectId == cityRefId)?.AssociatedObjectName
                            });
                        }
                    }
                }
            }

            return parents.SelectMany(x => x.Categories).ToArray();
        }

        private string ProductTypeToOutline(string productType)
        {
            switch (productType)
            {
                case RegionKey:
                    return ConfigurationManager.AppSettings["RegionCategoryId"];
                case EstateTypeKey:
                    return ConfigurationManager.AppSettings["EstateTypeCategoryId"];
                case TagsKey:
                    return ConfigurationManager.AppSettings["TagCategoryId"];
                case CitiesKey:
                    return ConfigurationManager.AppSettings["CityCategoryId"];
                case ConditionKey:
                    return ConfigurationManager.AppSettings["ConditionCategoryId"];
                case OtherTypeKey:
                    return ConfigurationManager.AppSettings["OtherTypeCategoryId"];
                case SinglePageKey:
                    return ConfigurationManager.AppSettings["SinglePageCategoryId"];
            }
            return string.Empty;
        }

        private string ProductPathToOutlineException(string productPath)
        {
            switch (productPath)
            {
                case "/Regions/Cities/Conditions":
                    return ConfigurationManager.AppSettings["CitiesConditionsCategoryId"];
                case "/Regions/Cities/Estatetypes/Conditions":
                    return ConfigurationManager.AppSettings["CitiesEstatetypesConditionsCategoryId"];
                case "/Regions/Cities/Estatetypes/Tags":
                    return ConfigurationManager.AppSettings["CitiesEstatetypesTagsCategoryId"];
                case "/Regions/Cities/Estatetypes":
                    return ConfigurationManager.AppSettings["CitiesEstatetypesCategoryId"];
                case "/Regions/Cities/Tags":
                    return ConfigurationManager.AppSettings["CitiesTagsCategoryId"];
                case "/Estatetypes/Tags":
                    return ConfigurationManager.AppSettings["EstatetypesTagsCategoryId"];
                case "/Estatetypes/Conditions":
                    return ConfigurationManager.AppSettings["ConditionEstatetypesCategoryId"];
                case "/Regions/Estatetypes":
                    return ConfigurationManager.AppSettings["RegionEstatetypeCategoryId"];
                case "/Regions/Tags":
                    return ConfigurationManager.AppSettings["RegionTagsCategoryId"];
                case "/Regions/Conditions":
                    return ConfigurationManager.AppSettings["RegionsConditionsCategoryId"];
                case "/Regions/Estatetypes/Conditions":
                    return ConfigurationManager.AppSettings["RegionsEstatetypesConditionsCategoryId"];
                case "/Regions/Estatetypes/Tags":
                    return ConfigurationManager.AppSettings["RegionsEstatetypesTagsCategoryId"];
                
                case "/Conditions/Tags":
                    return ConfigurationManager.AppSettings["ConditionTagsCategoryId"];

            }
            return string.Empty;
        }

        private Category ConvertProductToCategory(Category parent, string productType, AutoRestClients.CatalogModuleApi.Models.Product dtoProduct, List<Product> listExceptions, Dictionary<string, Category> seoDict)
        {
            var path = string.Join("/", parent.Path, productType);

            var converter = GetConverterByPath(path);

            var product = dtoProduct.ToProduct(_language, _currency, _store);

            var category = converter.ToCategory(new ConverterContext
                {
                    Parent = parent,
                    Path = path,
                    ProductType = productType,
                    ListExceptions = listExceptions
                }, product);
            if (category == null)
                return null;
            var seoPath = category.SeoPath.Trim('/');
            if (seoDict != null)
            {
                lock (seoDict)
                {
                    if (!seoDict.ContainsKey(seoPath))
                    {
                        seoDict.Add(seoPath, category);
                    }
                }
            }
            
            return category;
        }

        private async Task<List<Product>> GetExceptionProducts(string productPath)
        {
            var exceptionOutline = ProductPathToOutlineException(productPath);
            if (!string.IsNullOrEmpty(exceptionOutline))
            {
                var resultExceptions = await _catalogModuleApi.CatalogModuleSearch.SearchProductsAsync(
                    new AutoRestClients.CatalogModuleApi.Models.ProductSearchCriteria
                    {
                        CatalogId = ConfigurationManager.AppSettings["MasterCatalogId"],
                        ResponseGroup = "1619",
                        Outline = exceptionOutline,
                        Take = 10000,
                        Skip = 0,
                        WithHidden = false
                    });
                if (resultExceptions.Items != null)
                {
                    return resultExceptions.Items.Select(x => x.ToProduct(_language, _currency, _store)).ToList();
                }
            }
            return new List<Product>();
        }

        private ICategoryTreeConverter GetConverterByPath(string path)
        {
            if (path.EndsWith(TagsKey))
            {
                return new TagCategoryTreeConverter();
            }
            else if (path.EndsWith(EstateTypeKey))
            {
                return new TypeCategoryTreeConverter();
            }
            else if (path.EndsWith(CitiesKey))
            {
                return new CityCategoryTreeConverter();
            }
            else if (path.EndsWith(RegionKey))
            {
                return new RegionCategoryTreeConverter();
            }
            else if (path.EndsWith(OtherTypeKey))
            {
                return new OtherTypeCategoryTreeConverter();
            }
            else if (path.EndsWith(ConditionKey))
            {
                return new ConditionCategoryTreeConverter();
            }
            else if (path.EndsWith(SinglePageKey))
            {
                return new SinglePageCategoryTreeConverter();
            }
            return new DefaultCategoryTreeConverter();
        }

        public Category FindByPath(string path)
        {
            return _seoCategoryDict.ContainsKey(path) ? _seoCategoryDict[path] : null;
        }

        public Dictionary<string, Category> GetSeoDict()
        {
            return _seoCategoryDict;
        }

        public async Task ClearTree()
        {
            await _lockObject.WaitAsync();
            _seoCategoryDict = null;
            _lockObject.Release();
        }

        public async Task RebuildElement(string path)
        {
            // init data for product converter
            var wc = _workContextFactory();
            _language = wc.CurrentLanguage;
            _currency = wc.CurrentCurrency;
            _store = wc.CurrentStore;
            await _lockObject.WaitAsync();
            try
            {
                path = path.Trim('/');
                if (_seoCategoryDict.ContainsKey(path))
                {
                    var obj = _seoCategoryDict[path];
                    var productIds = new List<string>();
                    var categories = new List<Category>();
                    var current = obj;
                    do
                    {
                        productIds.Add(current.Id);
                        categories.Add(current);
                        current = current.Parent.Parent != null ? current.Parent : null;
                    } while (current != null);
                    var products = _catalogModuleApi.CatalogModuleProducts.GetProductByIds(productIds, CreateProductResponseGroup())
                        .Select(x => x.ToProduct(_language, _currency, _store));
                    categories.Reverse();
                    for (int i = 0; i < categories.Count; i++)
                    {
                        var category = categories[i];
                        var context = new ConverterContext
                        {
                            Parent = category.Parent,
                            Path = category.Path,
                            ProductType = category.ProductType
                        };
                        context.ListExceptions = await GetExceptionProducts(category.Path);
                        var convertCategory = GetConverterByPath(category.Path).ToCategory(context, products.First(x => x.Id == category.Id));
                        if (i + 1 < categories.Count)
                        {
                            categories[i + 1].Parent = convertCategory;
                        }
                        categories[i] = convertCategory;
                    }
                    lock (_seoCategoryDict)
                    {
                        _seoCategoryDict[path] = categories.Last();
                    }
                    ClearCache(new List<string>() { path });
                }
                else
                {
                    var pathItems = path.Split('/');
                    var categoryParent = new Category();
                    if (pathItems.Length > 1)
                    {
                        var parentPath = string.Join("/", pathItems.Take(pathItems.Length - 1));
                        if (_seoCategoryDict.ContainsKey(parentPath))
                        {
                            categoryParent = _seoCategoryDict[parentPath];
                        }
                    }
                    var seoRecords = GetAllSeoRecords(pathItems.LastOrDefault()).FirstOrDefault();
                    if (seoRecords != null)
                    {
                        var product = _catalogModuleApi.CatalogModuleProducts.GetProductById(seoRecords.ObjectId, CreateProductResponseGroup());
                        var productType = GetProductType(product.CategoryId);
                        if (productType == CitiesKey)
                        {
                            var regionId = product.Associations.FirstOrDefault(x => x.Type == "Regions")?.AssociatedObjectId;
                            if (!string.IsNullOrEmpty(regionId))
                            {
                                var region = _catalogModuleApi.CatalogModuleProducts.GetProductById(regionId, CreateProductResponseGroup());
                                categoryParent = ConvertProductToCategory(new Category(), RegionKey, region, null, null);
                            }
                            else
                            {
                                throw new Exception($"Not resolve region for city:{product.Name}");
                            }
                        }
                        var exceptionProducts = string.IsNullOrEmpty(categoryParent.Path) ? new List<Product>() : await GetExceptionProducts(string.Join("/", categoryParent.Path, productType));

                        ConvertProductToCategory(categoryParent, productType, product, exceptionProducts, _seoCategoryDict);
                        WorkContextOwinMiddleware.FilterSeo = null;
                    }
                }
            }
            finally
            {
                _lockObject.Release();
            }
        }

        private string GetProductType(string categoryId)
        {
            if (categoryId == ConfigurationManager.AppSettings["RegionCategoryId"])
            {
                return RegionKey;
            }
            else if (categoryId == ConfigurationManager.AppSettings["EstateTypeCategoryId"])
            {
                return EstateTypeKey;
            }
            else if (categoryId == ConfigurationManager.AppSettings["TagCategoryId"])
            {
                return TagsKey;
            }
            else if (categoryId == ConfigurationManager.AppSettings["CityCategoryId"])
            {
                return CitiesKey;
            }
            else if (categoryId == ConfigurationManager.AppSettings["ConditionCategoryId"])
            {
                return ConditionKey;
            }
            else if (categoryId == ConfigurationManager.AppSettings["OtherTypeCategoryId"])
            {
                return OtherTypeKey;
            }
            else if (categoryId == ConfigurationManager.AppSettings["SinglePageCategoryId"])
            {
                return SinglePageKey;
            }
            return string.Empty;
        }

        private string CreateProductResponseGroup()
        {
            return (ItemResponseGroup.ItemAssociations | ItemResponseGroup.Seo | ItemResponseGroup.ItemEditorialReviews | ItemResponseGroup.ItemInfo).ToString();
        }

        protected virtual IList<coreDto.SeoInfo> GetAllSeoRecords(string slug)
        {
            var result = new List<coreDto.SeoInfo>();

            if (!string.IsNullOrEmpty(slug))
            {
                var apiResult = _cacheManager.Get(string.Join(":", "Commerce.GetSeoInfoBySlug", slug), "ApiRegion", () => _coreApiFactory().Commerce.GetSeoInfoBySlug(slug));
                result.AddRange(apiResult);
            }

            return result;
        }

        private void ClearCache(List<string> keys)
        {
            //clear output cache
            try
            {
                var outputCacheKeys = keys.Select(x => $"{_store.Id}/{_language.CultureName.ToLower()}/{x}").ToList();
                var cacheHandle = CacheManager.Web.CacheManagerOutputCacheProvider.Cache.CacheHandles.FirstOrDefault();
                var cache = (System.Collections.IEnumerable)cacheHandle.GetType().GetField("cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cacheHandle);
                foreach (var item in cache)
                {
                    var key = item.GetType().GetProperty("Key", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(item).ToString();

                    if (outputCacheKeys.Any(x => key.Contains(x)))
                    {
                        var removeKey = string.Join(":", key.Split(':').Skip(1));
                        CacheManager.Web.CacheManagerOutputCacheProvider.Cache.Remove(removeKey);
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch
            {
                throw;
            }
            //clear seo request
            if (_cacheManager != null)
            {
                foreach (var key in keys)
                {
                    var paths = key.Split('/');
                    foreach (var path in paths)
                    {
                        _cacheManager.Remove($"Commerce.GetSeoInfoBySlug:{path}", "ApiRegion");
                    }
                }
            }
        }
    }
}
