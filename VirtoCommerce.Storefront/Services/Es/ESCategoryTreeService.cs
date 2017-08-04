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

namespace VirtoCommerce.Storefront.Services.Es
{
    public class ESCategoryTreeService : ICategoryTreeService
    {
        private readonly SemaphoreSlim _lockObject = new SemaphoreSlim(1);
        private readonly ICatalogModuleApiClient _catalogModuleApi;
        private readonly Func<WorkContext> _workContextFactory;
        private static Dictionary<string, Category> _seoCategoryDict;
        private static ConcurrentBag<Exception> _buildTreeExceptions = new ConcurrentBag<Exception>();
        private Language _language;
        private Currency _currency;
        private Store _store;
        private readonly ILocalCacheManager _cacheManager;

        private const string RegionKey = "Regions";
        private const string EstateTypeKey = "Estatetypes";
        private const string TagsKey = "Tags";
        private const string ConditionKey = "Conditions";
        private const string OtherTypeKey = "OtherType";
        private const string CitiesKey = "Cities";
        private const string SinglePageKey = "SinglePage";

        public ESCategoryTreeService(ICatalogModuleApiClient catalogModuleApi, Func<WorkContext> workContextFactory, ILocalCacheManager cacheManager)
        {
            _catalogModuleApi = catalogModuleApi;
            _workContextFactory = workContextFactory;
            _cacheManager = cacheManager;

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
                return await GenerateTree();
            }
            finally
            {
                _lockObject.Release();
            }
        }

        public async Task<Dictionary<string, Category>> BuildTree()
        {
            try
            {
                await _lockObject.WaitAsync();
                return await GenerateTree();
            }
            finally
            {
                _lockObject.Release();
            }
        }

        private async Task<Dictionary<string, Category>> GenerateTree()
        {
            var tempSeoDict = new Dictionary<string, Category>();
            if (_cacheManager != null)
            {
                _cacheManager.Clear();
            }
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
                var exceptionOutline = ProductPathToOutlineException(string.Join("/", firstParent.Path, productType));
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
                        listExceptions = resultExceptions.Items.Select(x => x.ToProduct(_language, _currency, _store)).ToList();
                    }
                }
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
                   ResponseGroup = (ItemResponseGroup.ItemAssociations | ItemResponseGroup.Seo | ItemResponseGroup.ItemEditorialReviews | ItemResponseGroup.ItemInfo).ToString(),
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
                    var children = resultCities.Items.Where(x => x != null && x.Associations != null && x.Associations.Any(a => a.AssociatedObjectId == parent.Id)).Select(p => ConvertProductToCategory(parent, "Cities", p, new List<Product>(), seoDict)).Where(x => x != null).ToList();
                    categories.AddRange(children);
                }
                parent.Categories = new MutablePagedList<Category>(categories);
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
            lock (seoDict)
            {
                if (!seoDict.ContainsKey(seoPath))
                {
                    seoDict.Add(seoPath, category);
                }
            }
            
            return category;
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
            await _lockObject.WaitAsync();
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
                var products = _catalogModuleApi.CatalogModuleProducts.GetProductByIds(productIds, 
                    (ItemResponseGroup.ItemAssociations | ItemResponseGroup.Seo | ItemResponseGroup.ItemEditorialReviews | ItemResponseGroup.ItemInfo).ToString())
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
                    var exceptionOutline = ProductPathToOutlineException(category.Path);
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
                            context.ListExceptions = resultExceptions.Items.Select(x => x.ToProduct(_language, _currency, _store)).ToList();
                        }
                    }
                    var convertCategory = GetConverterByPath(category.Path).ToCategory(context, products.First(x => x.Id == category.Id));
                    if (i + 1 < categories.Count)
                    {
                        categories[i + 1].Parent = convertCategory;
                    }
                }
                if (_cacheManager != null)
                {
                    _cacheManager.Clear();
                    //_cacheManager.Remove($"SeoProducts:Product{obj.Id}");
                }
            }
            _lockObject.Release();
        }
    }
}