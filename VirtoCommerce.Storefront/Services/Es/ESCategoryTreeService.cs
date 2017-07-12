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

namespace VirtoCommerce.Storefront.Services.Es
{
    public class ESCategoryTreeService : ICategoryTreeService
    {
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
            // Load links for regions
            //await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions");
            // Load region and regio + estate type
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions").ContinueWith(t => LoadChildrenFromCategory(t.Result, "Estatetypes"));
            // Load region + tags
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions").ContinueWith(t => LoadChildrenFromCategory(t.Result, "Tags"));
            // Load region + condition
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions").ContinueWith(t => LoadChildrenFromCategory(t.Result, "Conditions"));
            // Load region + estate type + tags
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions").ContinueWith(t => LoadChildrenFromCategory(t.Result, "Estatetypes").ContinueWith(t1 => LoadChildrenFromCategory(t1.Result, "Tags")));
            // Load region + estate type + condition
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions").ContinueWith(t => LoadChildrenFromCategory(t.Result, "Estatetypes").ContinueWith(t1 => LoadChildrenFromCategory(t1.Result, "Conditions")));

            // Load cities + estate type + tags
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions").ContinueWith(t1 => LoadCities(t1.Result).ContinueWith(t => LoadChildrenFromCategory(t.Result, "Estatetypes").ContinueWith(t2 => LoadChildrenFromCategory(t2.Result, "Tags"))));

            // Load cities + tags
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions").ContinueWith(t1 => LoadCities(t1.Result).ContinueWith(t => LoadChildrenFromCategory(t.Result, "Tags")));

            // Load cities + estate type + conditions
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Regions").ContinueWith(t1 => LoadCities(t1.Result).ContinueWith(t => LoadChildrenFromCategory(t.Result, "Estatetypes").ContinueWith(t2 => LoadChildrenFromCategory(t2.Result, "Conditions"))));

            // Step 2. Load Estatetypes
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Estatetypes");

            // Step 3. Load Tags
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Tags");

            // Step 4. Load Conditions
            await LoadChildrenFromCategory(new Category[] { new Category() }, "Conditions");
            // Step 5. Load Other type
            await LoadChildrenFromCategory(new Category[] { new Category() }, "OtherType");
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
                            Skip = 0
                        });
                    listExceptions = resultExceptions.Items.Select(x => x.ToProduct(_language, _currency, _store)).ToList();
                }
            }
            foreach (var parent in parents)
            {
                var categories = new List<Category>();

                if (result.Items != null)
                {

                    var children = result.Items.Select(p => ConvertProductToCategory(parent, productType, p, listExceptions));
                    if (parent.Categories != null)
                        categories.AddRange(parent.Categories);
                    categories.AddRange(children);
                }

                parent.Categories = new MutablePagedList<Category>(categories);
            }

            return parents.SelectMany(p => p.Categories).ToArray();
        }

        private async Task<Category[]> LoadCities(Category[] parents)
        {
            var resultCities = await _catalogModuleApi.CatalogModuleSearch.SearchProductsAsync(
               new AutoRestClients.CatalogModuleApi.Models.ProductSearchCriteria
               {
                   CatalogId = ConfigurationManager.AppSettings["MasterCatalogId"],
                   ResponseGroup = (ItemResponseGroup.ItemAssociations | ItemResponseGroup.Seo | ItemResponseGroup.ItemEditorialReviews | ItemResponseGroup.ItemInfo).ToString(),
                   Outline = ProductTypeToOutline("Cities"),
                   Take = 10000,
                   Skip = 0
               });
            foreach (var parent in parents)
            {
                var categories = new List<Category>();

                if (resultCities.Items != null)
                {
                    var children = resultCities.Items.Where(x => x.Associations!= null && x.Associations.Any(a => a.AssociatedObjectId == parent.Id)).Select(p => ConvertProductToCategory(parent, "Cities", p, new List<Product>())).ToList();
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
                case "Regions":
                    return ConfigurationManager.AppSettings["RegionCategoryId"];
                case "Estatetypes":
                    return ConfigurationManager.AppSettings["EstateTypeCategoryId"];
                case "Tags":
                    return ConfigurationManager.AppSettings["TagCategoryId"];
                case "Cities":
                    return ConfigurationManager.AppSettings["CityCategoryId"];
                case "Conditions":
                    return ConfigurationManager.AppSettings["ConditionCategoryId"];
                case "OtherType":
                    return ConfigurationManager.AppSettings["OtherTypeCategoryId"];
            }
            return string.Empty;
        }

        private string ProductPathToOutlineException(string productPath)
        {
            switch (productPath)
            {
                case "/Cities/Conditions":
                    return ConfigurationManager.AppSettings["CitiesConditionsCategoryId"];
                case "/Cities/Estatetypes/Conditions":
                    return ConfigurationManager.AppSettings["CitiesEstatetypesConditionsCategoryId"];
                case "/Cities/Estatetypes/Tags":
                    return ConfigurationManager.AppSettings["CitiesEstatetypesTagsCategoryId"];
                case "/Cities/Tags":
                    return ConfigurationManager.AppSettings["CitiesTagsCategoryId"];
                case "/Estatetypes/Tags":
                    return ConfigurationManager.AppSettings["EstatetypesTagsCategoryId"];
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
            }
            return string.Empty;
        }

        private Category ConvertProductToCategory(Category parent, string productType, AutoRestClients.CatalogModuleApi.Models.Product dtoProduct, List<Product> listExceptions)
        {
            var path = string.Join("/", parent.Path, productType);

            // TODO: Find Converter By Seo Path
            var converter = GetConverterByPath(path);

            var product = dtoProduct.ToProduct(_language, _currency, _store);

            var category = converter.ToCategory(new ConverterContext
                {
                    Parent = parent,
                    Path = path,
                    ProductType = productType,
                    ListExceptions = listExceptions
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
            if (path.EndsWith("Tag"))
            {
                return new TagCategoryTreeConverter();
            }
            else if (path.EndsWith("Estatetypes"))
            {
                return new TypeCategoryTreeConverter();
            }
            else if (path.EndsWith("Cities"))
            {
                return new CityCategoryTreeConverter();
            }
            else if (path.EndsWith("Regions"))
            {
                return new RegionCategoryTreeConverter();
            }
            else if (path.EndsWith("OtherType"))
            {
                return new OtherTypeCategoryTreeConverter();
            }
            else if (path.EndsWith("Conditions"))
            {
                return new ConditionCategoryTreeConverter();
            }
            return new DefaultCategoryTreeConverter();
        }

        public Category FindByPath(string path)
        {
            return _seoCategoryDict.ContainsKey(path) ? _seoCategoryDict[path] : null;
        }
    }
}