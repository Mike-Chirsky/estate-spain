using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi.Models;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.AutoRestClients.Es
{
    public class EsPricingModule : IPricingModule
    {
        private readonly IPricingModule _pricingModule;
        private readonly ILocalCacheManager _cacheManager;
        private readonly bool _cacheEnabled;

        public EsPricingModule(IPricingModule pricingModule, ILocalCacheManager cacheManager)
        {
            _pricingModule = pricingModule;
            _cacheManager = cacheManager;
            _cacheEnabled = ConfigurationManager.AppSettings.GetValue("VirtoCommerce:Storefront:ApiRequest:Pricing:CacheEnabled", true);
        }

        public Task<HttpOperationResponse<PricelistAssignment>> CreatePricelistAssignmentWithHttpMessagesAsync(PricelistAssignment assignment, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.CreatePricelistAssignmentWithHttpMessagesAsync(assignment, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<Pricelist>> CreatePriceListWithHttpMessagesAsync(Pricelist priceList, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.CreatePriceListWithHttpMessagesAsync(priceList, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse> DeleteAssignmentsWithHttpMessagesAsync(IList<string> ids, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.DeleteAssignmentsWithHttpMessagesAsync(ids, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse> DeletePricelistsWithHttpMessagesAsync(IList<string> ids, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.DeletePricelistsWithHttpMessagesAsync(ids, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse> DeleteProductPricesWithHttpMessagesAsync(string pricelistId, IList<string> productIds, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.DeleteProductPricesWithHttpMessagesAsync(pricelistId, productIds, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<IList<Pricelist>>> EvaluatePriceListsWithHttpMessagesAsync(PriceEvaluationContext evalContext, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_cacheEnabled)
            {
                var cacheKey = GetContextCacheKey("EvaluatePriceLists", evalContext);

                var retVal = _cacheManager.GetAsync(cacheKey, "ApiRegion", async () =>
                {
                    return await _pricingModule.EvaluatePriceListsWithHttpMessagesAsync(evalContext, customHeaders, cancellationToken);
                });

                return retVal;
            }

            return _pricingModule.EvaluatePriceListsWithHttpMessagesAsync(evalContext, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<IList<Price>>> EvaluatePricesWithHttpMessagesAsync(PriceEvaluationContext evalContext, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_cacheEnabled)
            {
                var cacheKey = GetContextCacheKey("EvaluatePrices", evalContext);

                var retVal = _cacheManager.GetAsync(cacheKey, "ApiRegion", async () =>
                {
                    return await _pricingModule.EvaluatePricesWithHttpMessagesAsync(evalContext, customHeaders, cancellationToken);
                });

                return retVal;
            }

            return _pricingModule.EvaluatePricesWithHttpMessagesAsync(evalContext, customHeaders, cancellationToken);

        }

        public Task<HttpOperationResponse<IList<Price>>> EvaluateProductPricesWithHttpMessagesAsync(string productId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_cacheEnabled)
            {
                var cacheKey = string.Join("-", "EvaluateProductPrices", productId);

                var retVal = _cacheManager.GetAsync(cacheKey, "ApiRegion", async () =>
                {
                    return await _pricingModule.EvaluateProductPricesWithHttpMessagesAsync(productId, customHeaders, cancellationToken);
                });

                return retVal;
            }

            return _pricingModule.EvaluateProductPricesWithHttpMessagesAsync(productId, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<PricelistAssignment>> GetNewPricelistAssignmentsWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.GetNewPricelistAssignmentsWithHttpMessagesAsync(customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<PricelistAssignment>> GetPricelistAssignmentByIdWithHttpMessagesAsync(string id, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.GetPricelistAssignmentByIdWithHttpMessagesAsync(id, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<Pricelist>> GetPriceListByIdWithHttpMessagesAsync(string id, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.GetPriceListByIdWithHttpMessagesAsync(id, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<IList<Pricelist>>> GetProductPriceListsWithHttpMessagesAsync(string productId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.GetProductPriceListsWithHttpMessagesAsync(productId, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<PricingSearchResultPricelistAssignment>> SearchPricelistAssignmentsWithHttpMessagesAsync(string criteriapriceListId = null, IList<string> criteriapriceListIds = null, string criteriakeyword = null, string criteriasort = null, int? criteriaskip = default(int?), int? criteriatake = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.SearchPricelistAssignmentsWithHttpMessagesAsync(criteriapriceListId, criteriapriceListIds, criteriakeyword, criteriasort, criteriaskip, criteriatake, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<PricingSearchResultPricelist>> SearchPricelistsWithHttpMessagesAsync(string criteriakeyword = null, string criteriasort = null, int? criteriaskip = default(int?), int? criteriatake = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.SearchPricelistsWithHttpMessagesAsync(criteriakeyword, criteriasort, criteriaskip, criteriatake, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse<PricingSearchResultProductPrice>> SearchProductPricesWithHttpMessagesAsync(bool? criteriagroupByProducts = default(bool?), string criteriapriceListId = null, IList<string> criteriapriceListIds = null, string criteriaproductId = null, IList<string> criteriaproductIds = null, string criteriakeyword = null, string criteriasort = null, int? criteriaskip = default(int?), int? criteriatake = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.SearchProductPricesWithHttpMessagesAsync(criteriagroupByProducts, criteriapriceListId, criteriapriceListIds, criteriaproductId, criteriaproductIds, criteriakeyword, criteriasort, criteriaskip, criteriatake, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse> UpdatePriceListAssignmentWithHttpMessagesAsync(PricelistAssignment assignment, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.UpdatePriceListAssignmentWithHttpMessagesAsync(assignment, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse> UpdatePriceListWithHttpMessagesAsync(Pricelist priceList, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.UpdatePriceListWithHttpMessagesAsync(priceList, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse> UpdateProductPricesWithHttpMessagesAsync(ProductPrice productPrice, string productId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.UpdateProductPricesWithHttpMessagesAsync(productPrice, productId, customHeaders, cancellationToken);
        }

        public Task<HttpOperationResponse> UpdateProductsPricesWithHttpMessagesAsync(IList<ProductPrice> productPrices, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _pricingModule.UpdateProductsPricesWithHttpMessagesAsync(productPrices, customHeaders, cancellationToken);
        }

        private String GetContextCacheKey(string method, PriceEvaluationContext context)
        {
            var cacheKey = string.Join("-",
                method,
                context.CatalogId,
                context.Currency ?? string.Empty,
                context.StoreId,
                context.ProductIds != null ? string.Join("-", context.ProductIds.ToArray()) : string.Empty,
                string.Join("-", context.PricelistIds.ToArray()));

            return cacheKey;
        }
    }
}