﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PagedList;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Converters.Subscriptions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Services;
using catalogDto = VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi.Models;

namespace VirtoCommerce.Storefront.Services
{
    public class CatalogSearchServiceImpl : ICatalogSearchService
    {
        private readonly Func<WorkContext> _workContextFactory;
        private readonly ICatalogModuleApiClient _catalogModuleApi;
        private readonly IInventoryModuleApiClient _inventoryModuleApi;
        private readonly IPricingService _pricingService;
        private readonly ICustomerService _customerService;
        private readonly ISubscriptionModuleApiClient _subscriptionApi;
        private readonly IProductAvailabilityService _productAvailabilityService;

        public CatalogSearchServiceImpl(
            Func<WorkContext> workContextFactory,
            ICatalogModuleApiClient catalogModuleApi,
            IInventoryModuleApiClient inventoryModuleApi,
            IPricingService pricingService,
            ICustomerService customerService,
            ISubscriptionModuleApiClient subscriptionApi,
            IProductAvailabilityService productAvailabilityService)
        {
            _workContextFactory = workContextFactory;
            _catalogModuleApi = catalogModuleApi;
            _pricingService = pricingService;
            _inventoryModuleApi = inventoryModuleApi;
            _customerService = customerService;
            _subscriptionApi = subscriptionApi;
            _productAvailabilityService = productAvailabilityService;
        }

        #region ICatalogSearchService Members

        public virtual async Task<Product[]> GetProductsAsync(string[] ids, ItemResponseGroup responseGroup = ItemResponseGroup.None)
        {
            Product[] result;

            if (ids.IsNullOrEmpty())
            {
                result = new Product[0];
            }
            else
            {
                var workContext = _workContextFactory();

                if (responseGroup == ItemResponseGroup.None)
                {
                    responseGroup = workContext.CurrentProductResponseGroup;
                }

                result = await GetProductsAsync(ids, responseGroup, workContext);

                var allProducts = result.Concat(result.SelectMany(p => p.Variations)).ToList();

                if (!allProducts.IsNullOrEmpty())
                {
                    var taskList = new List<Task>();

                    if (responseGroup.HasFlag(ItemResponseGroup.ItemAssociations))
                    {
                        taskList.Add(LoadProductAssociationsAsync(allProducts, responseGroup));
                    }

                    if (responseGroup.HasFlag(ItemResponseGroup.Inventory))
                    {
                        taskList.Add(LoadProductInventoriesAsync(allProducts));
                    }

                    if (responseGroup.HasFlag(ItemResponseGroup.ItemWithPrices))
                    {
                        taskList.Add(_pricingService.EvaluateProductPricesAsync(allProducts, workContext));
                    }

                    if (responseGroup.HasFlag(ItemResponseGroup.ItemWithVendor))
                    {
                        taskList.Add(LoadProductVendorsAsync(allProducts, workContext));
                    }

                    if (workContext.CurrentStore.SubscriptionEnabled && responseGroup.HasFlag(ItemResponseGroup.ItemWithPaymentPlan))
                    {
                        taskList.Add(LoadProductPaymentPlanAsync(allProducts, workContext));
                    }

                    await Task.WhenAll(taskList.ToArray());

                    foreach (var product in allProducts)
                    {
                        product.IsAvailable = await _productAvailabilityService.IsAvailable(product, 1);
                    }
                }
            }

            return result;
        }

        public virtual Category[] GetCategories(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            var workContext = _workContextFactory();
            return Task.Factory.StartNew(() => InnerGetCategoriesAsync(ids, workContext, responseGroup), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public virtual async Task<Category[]> GetCategoriesAsync(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            var workContext = _workContextFactory();
            return await InnerGetCategoriesAsync(ids, workContext, responseGroup);
        }

        /// <summary>
        /// Async search categories by given criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<Category>> SearchCategoriesAsync(CategorySearchCriteria criteria)
        {
            var workContext = _workContextFactory();
            return await InnerSearchCategoriesAsync(criteria, workContext);
        }

        /// <summary>
        /// Search categories by given criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual IPagedList<Category> SearchCategories(CategorySearchCriteria criteria)
        {
            var workContext = _workContextFactory();
            return Task.Factory.StartNew(() => InnerSearchCategoriesAsync(criteria, workContext), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Async search products by given criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual async Task<CatalogSearchResult> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            var workContext = _workContextFactory();
            return await InnerSearchProductsAsync(criteria, workContext);
        }

        /// <summary>
        /// Search products by given criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual CatalogSearchResult SearchProducts(ProductSearchCriteria criteria)
        {
            var workContext = _workContextFactory();
            return Task.Factory.StartNew(() => InnerSearchProductsAsync(criteria, workContext), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        #endregion


        protected virtual async Task<Product[]> GetProductsAsync(IList<string> ids, ItemResponseGroup responseGroup, WorkContext workContext)
        {
            var productDtos = await _catalogModuleApi.CatalogModuleProducts.GetProductByPlentyIdsAsync(ids, ((int)responseGroup).ToString());

            var result = productDtos.Select(x => x.ToProduct(workContext.CurrentLanguage, workContext.CurrentCurrency, workContext.CurrentStore)).ToArray();
            return result;
        }

        private async Task<Category[]> InnerGetCategoriesAsync(string[] ids, WorkContext workContext, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            var retVal = (await _catalogModuleApi.CatalogModuleCategories.GetCategoriesByPlentyIdsAsync(ids.ToList(), ((int)responseGroup).ToString())).Select(x => x.ToCategory(workContext.CurrentLanguage, workContext.CurrentStore)).ToArray();
            //Set  lazy loading for child categories 
            SetChildCategoriesLazyLoading(retVal);
            return retVal;
        }

        private async Task<IPagedList<Category>> InnerSearchCategoriesAsync(CategorySearchCriteria criteria, WorkContext workContext)
        {
            criteria = criteria.Clone();
            var searchCriteria = criteria.ToCategorySearchCriteriaDto(workContext);
            var result = await _catalogModuleApi.CatalogModuleSearch.SearchCategoriesAsync(searchCriteria);

            var retVal = new PagedList<Category>(new List<Category>(), 1, 1);
            if (result.Items != null)
            {
                retVal = new PagedList<Category>(result.Items.Select(x => x.ToCategory(workContext.CurrentLanguage, workContext.CurrentStore)), criteria.PageNumber, criteria.PageSize);
            }

            //Set  lazy loading for child categories 
            SetChildCategoriesLazyLoading(retVal.ToArray());
            return retVal;
        }

        private async Task<CatalogSearchResult> InnerSearchProductsAsync(ProductSearchCriteria criteria, WorkContext workContext)
        {
            criteria = criteria.Clone();

            var searchCriteria = criteria.ToProductSearchCriteriaDto(workContext);
            var result = await _catalogModuleApi.CatalogModuleSearch.SearchProductsAsync(searchCriteria);
            //await LoadAllValuesForSelectedTerms(criteria, workContext, result);
            // TODO Remove check is null afeter fix null values from admin
            var products = result.Items?.Where(x => x != null).Select(x => x.ToProduct(workContext.CurrentLanguage, workContext.CurrentCurrency, workContext.CurrentStore)).ToList() ?? new List<Product>();

            if (products.Any())
            {
                var productsWithVariations = products.Concat(products.SelectMany(x => x.Variations)).ToList();
                var taskList = new List<Task>();

                if (criteria.ResponseGroup.HasFlag(ItemResponseGroup.ItemAssociations))
                {
                    taskList.Add(LoadProductAssociationsAsync(productsWithVariations, criteria.AssociationsResponseGroup == ItemResponseGroup.None? criteria.ResponseGroup : criteria.AssociationsResponseGroup));
                }

                if (criteria.ResponseGroup.HasFlag(ItemResponseGroup.Inventory))
                {
                    taskList.Add(LoadProductInventoriesAsync(productsWithVariations));
                }

                if (criteria.ResponseGroup.HasFlag(ItemResponseGroup.ItemWithVendor))
                {
                    taskList.Add(LoadProductVendorsAsync(productsWithVariations, workContext));
                }

                if (criteria.ResponseGroup.HasFlag(ItemResponseGroup.ItemWithPrices))
                {
                    taskList.Add(_pricingService.EvaluateProductPricesAsync(productsWithVariations, workContext));
                }

                await Task.WhenAll(taskList.ToArray());

                foreach (var product in productsWithVariations)
                {
                    product.IsAvailable = await _productAvailabilityService.IsAvailable(product, 1);
                }
            }

            return new CatalogSearchResult
            {
                Products = new StaticPagedList<Product>(products, criteria.PageNumber, criteria.PageSize, (int?)result.TotalCount ?? 0),
                Aggregations = !result.Aggregations.IsNullOrEmpty() ? result.Aggregations.Select(x => x.ToAggregation(workContext.CurrentLanguage.CultureName)).ToArray() : new Aggregation[] { }
            };
        }

        private async Task LoadAllValuesForSelectedTerms(ProductSearchCriteria criteria, WorkContext workContext, catalogDto.ProductSearchResult result)
        {
            // OZ: Don't load for brand and tags page
            if (criteria.Terms != null)
            {
                var termNames = criteria.Terms.Select(s => s.Name).Distinct().ToArray();

                if (termNames.Any())
                {
                    var tasks = new List<Task>();

                    foreach (var termName in termNames)
                    {
                        // Load Only Aggregations without Selected Term and Products
                        var allAggrSearchCriteria = criteria.ToProductSearchCriteriaDto(workContext);
                        allAggrSearchCriteria.Take = 0;
                        var updatedTerms = allAggrSearchCriteria.Terms.Where(i => !i.StartsWith(termName + ":")).ToList();
                        allAggrSearchCriteria.Terms = updatedTerms;

                        var task = _catalogModuleApi.CatalogModuleSearch.SearchProductsAsync(allAggrSearchCriteria)
                            .ContinueWith(t => CopyAggregations(termName, t.Result, result));

                        tasks.Add(task);
                    }

                    await Task.WhenAll(tasks);
                }
            }
        }

        private static void CopyAggregations(string termName, catalogDto.ProductSearchResult srcResult, catalogDto.ProductSearchResult destResult)
        {
            var srcAggregation = srcResult.Aggregations?.FirstOrDefault(i => i.Field == termName);
            if (srcAggregation != null)
            {
                if (destResult.Aggregations == null)
                    destResult.Aggregations = new List<catalogDto.Aggregation>();

                var destAggregation = destResult.Aggregations.FirstOrDefault(i => i.Field == termName);
                if (destAggregation != null)
                    CopyAggregations(srcAggregation, destAggregation);
                else
                    destResult.Aggregations.Add(srcAggregation);
            }
        }

        private static void CopyAggregations(catalogDto.Aggregation srcAggregation, catalogDto.Aggregation destAggregation)
        {
            if (srcAggregation == null || destAggregation == null)
                return;

            var newItems = srcAggregation.Items.Where(i => destAggregation.Items.FirstOrDefault(j => string.Equals(j.Value as string, i.Value as string)) == null);

            destAggregation.Items.AddRange(newItems);
        }

        protected virtual async Task LoadProductVendorsAsync(List<Product> products, WorkContext workContext)
        {
            var vendorIds = products.Where(p => !string.IsNullOrEmpty(p.VendorId)).Select(p => p.VendorId).Distinct().ToArray();
            if (!vendorIds.IsNullOrEmpty())
            {
                var vendors = await _customerService.GetVendorsByIdsAsync(workContext.CurrentStore, workContext.CurrentLanguage, vendorIds);
                foreach (var product in products)
                {
                    product.Vendor = vendors.FirstOrDefault(v => v != null && v.Id == product.VendorId);
                    if (product.Vendor != null)
                    {
                        product.Vendor.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos) =>
                        {
                            var criteria = new ProductSearchCriteria
                            {
                                VendorId = product.VendorId,
                                PageNumber = pageNumber,
                                PageSize = pageSize,
                                ResponseGroup = workContext.CurrentProductSearchCriteria.ResponseGroup & ~ItemResponseGroup.ItemWithVendor,
                                SortBy = SortInfo.ToString(sortInfos),
                            };

                            var searchResult = SearchProducts(criteria);
                            return searchResult.Products;
                        }, 1, ProductSearchCriteria.DefaultPageSize);
                    }
                }
            }
        }


        protected virtual async Task LoadProductAssociationsAsync(IEnumerable<Product> products, ItemResponseGroup productResponseGroup = ItemResponseGroup.None)
        {
            var allAssociations = products.SelectMany(x => x.Associations).ToList();

            var allProductAssociations = allAssociations.OfType<ProductAssociation>().ToList();
            var allCategoriesAssociations = allAssociations.OfType<CategoryAssociation>().ToList();

            if (allProductAssociations.Any())
            {
                var allAssociatedProducts = await GetProductsAsync(allProductAssociations.Select(x => x.ProductId).ToArray(), productResponseGroup);

                foreach (var productAssociation in allProductAssociations)
                {
                    productAssociation.Product = allAssociatedProducts.FirstOrDefault(x => x.Id == productAssociation.ProductId);
                }
            }

            if (allCategoriesAssociations.Any())
            {
                var allAssociatedCategories = await GetCategoriesAsync(allCategoriesAssociations.Select(x => x.CategoryId).ToArray(), CategoryResponseGroup.Info | CategoryResponseGroup.WithSeo | CategoryResponseGroup.WithOutlines | CategoryResponseGroup.WithImages);

                foreach (var categoryAssociation in allCategoriesAssociations)
                {
                    categoryAssociation.Category = allAssociatedCategories.FirstOrDefault(x => x.Id == categoryAssociation.CategoryId);

                    if (categoryAssociation.Category != null && categoryAssociation.Category.Products == null)
                    {
                        categoryAssociation.Category.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos) =>
                        {
                            var criteria = new ProductSearchCriteria
                            {
                                PageNumber = pageNumber,
                                PageSize = pageSize,
                                Outline = categoryAssociation.Category.Outline,
                                ResponseGroup = ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemWithPrices
                            };

                            if (!sortInfos.IsNullOrEmpty())
                            {
                                criteria.SortBy = SortInfo.ToString(sortInfos);
                            }

                            var searchResult = SearchProducts(criteria);
                            return searchResult.Products;
                        }, 1, ProductSearchCriteria.DefaultPageSize);
                    }
                }
            }
        }

        protected virtual async Task LoadProductInventoriesAsync(List<Product> products)
        {
            var inventories = await _inventoryModuleApi.InventoryModule.GetProductsInventoriesByPlentyIdsAsync(products.Select(x => x.Id).ToArray());

            foreach (var item in products)
            {
                item.Inventory = inventories.Where(x => x.ProductId == item.Id).Select(x => x.ToInventory()).FirstOrDefault();
            }
        }

        protected virtual void LoadProductInventories(List<Product> products)
        {
            var inventories = _inventoryModuleApi.InventoryModule.GetProductsInventoriesByPlentyIds(products.Select(x => x.Id).ToArray());

            foreach (var item in products)
            {
                item.Inventory = inventories.Where(x => x.ProductId == item.Id).Select(x => x.ToInventory()).FirstOrDefault();
            }
        }

        protected virtual async Task LoadProductPaymentPlanAsync(List<Product> products, WorkContext workContext)
        {
            var paymentPlans = await _subscriptionApi.SubscriptionModule.GetPaymentPlanByIdsAsync(products.Select(x => x.Id).ToArray());
            foreach (var product in products)
            {
                product.PaymentPlan = paymentPlans.Where(x => x.Id == product.Id).Select(x => x.ToPaymentPlan()).FirstOrDefault();
            }
        }

        protected virtual void SetChildCategoriesLazyLoading(Category[] categories)
        {
            foreach (var category in categories)
            {
                //Lazy loading for parents categories
                category.Parents = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos) =>
                {
                    var catIds = category.Outline.Split('/');
                    return new StaticPagedList<Category>(GetCategories(catIds, CategoryResponseGroup.Small), pageNumber, pageSize, catIds.Length);
                }, 1, CategorySearchCriteria.DefaultPageSize);

                //Lazy loading for child categories
                category.Categories = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos) =>
                {
                    var categorySearchCriteria = new CategorySearchCriteria
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Outline = "/" + category.Outline
                    };
                    if (!sortInfos.IsNullOrEmpty())
                    {
                        categorySearchCriteria.SortBy = SortInfo.ToString(sortInfos);
                    }
                    var searchResult = SearchCategories(categorySearchCriteria);
                    return searchResult;
                }, 1, CategorySearchCriteria.DefaultPageSize);
            }
        }

    }
}